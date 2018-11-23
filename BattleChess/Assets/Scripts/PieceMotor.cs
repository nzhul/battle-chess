using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Piece))]
public class PieceMotor : MonoBehaviour
{
    public Vector3 destination;

    public bool faceDestination = true;

    public bool isMoving = false;

    public bool isRotating = false;

    public iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;

    public float moveSpeed = 1.5f;

    public float rotateTime = 0.5f;

    public float iTweenDelay = 0f;

    private Piece piece;

    public event Action<Piece> OnMovementComplete;

    private void Awake()
    {
        this.piece = GetComponent<Piece>();
    }

    public void Move(Vector3 destinationPos, float delay = 0.25f)
    {
        if (isMoving)
        {
            return;
        }

        if (BoardManager.Instance != null)
        {
            StartCoroutine(MoveRoutine(destinationPos, delay));
        }
    }

    protected virtual IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        isMoving = true;
        PlayerManager.Instance.InputEnabled = false;

        // set the destination to the destinationPos being passed into the coroutine
        destination = destinationPos;

        // optional turn to face destination
        if (faceDestination)
        {
            FaceTarget(destination);
            yield return new WaitForSeconds(0.25f);
        }

        // pause the coroutine for a brief periof
        yield return new WaitForSeconds(delayTime);

        // move toward the destinationPos using the easeType and moveSpeed variables
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easetype", easeType,
            "speed", moveSpeed
        ));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01f)
        {
            yield return null;
        }

        // stop the iTween immediately
        iTween.Stop(gameObject);

        // set our position to the destination explicitly
        transform.position = destinationPos;

        // we are not moving
        isMoving = false;
        PlayerManager.Instance.InputEnabled = true;
        this.piece.WalkConsumed = true;

        this.InvokeOnMovementComplete();
    }

    public void InvokeOnMovementComplete()
    {
        if (this.OnMovementComplete != null)
        {
            this.OnMovementComplete(this.piece);
        }
    }

    public void FaceTarget(Vector3 target)
    {
        StartCoroutine(FaceTargetRoutine(target));
    }

    IEnumerator FaceTargetRoutine(Vector3 target)
    {
        isRotating = true;

        // direction to destination
        Vector3 relativePosition = target - transform.position;

        // vector direction converted to a Quaternion rotation
        Quaternion newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);

        // euler angle y component 
        float newY = newRotation.eulerAngles.y;

        // iTween rotate
        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", easeType,
            "time", rotateTime
        ));

        while (!LookingAtTarget(target))
        {
            yield return null;
        }

        isRotating = false;
    }

    private bool LookingAtTarget(Vector3 target)
    {
        Vector3 dirFromTarget = (target - transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromTarget, transform.forward);

        return dotProd >= 0.95;
    }
}