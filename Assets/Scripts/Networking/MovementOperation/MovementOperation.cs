using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum OperationType
{
    Pickup,
    Move,
    Drop
}
public class MovementOperation : MonoBehaviour
{
    public delegate void HandleSuccess();
    public delegate void HandleFail();
    public delegate void HandleCancel();

    public event HandleSuccess OnOperationSuccess;
    public event HandleFail OnOperationFail;
    public event HandleCancel OnOperationCanceled;

    public MovementOperationData data;


    private static long nextId = 0;
    private static List<MovementOperation> operations = new List<MovementOperation>();

    public struct MovementOperationData : INetworkSerializable
    {
        public long operationId;
        public OperationType type;
        public long sourceId;
        public long destinationId;
        public bool shifting;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref operationId);
            serializer.SerializeValue(ref type);
            serializer.SerializeValue(ref sourceId);
            serializer.SerializeValue(ref destinationId);
            serializer.SerializeValue(ref shifting);
        }
    }


    //Picking Up
    public MovementOperation(long sourceId, long destinationId, bool shifting)
    {
        data = new MovementOperationData
        {
            operationId = nextId,
            type = OperationType.Pickup,
            sourceId = sourceId,
            destinationId = destinationId,
            shifting = shifting
        };
        operations.Add(this);
        nextId++;
    }

    //Used for dropping and moving
    public MovementOperation(OperationType type, long sourceId, long destinationId, bool shifting)
    {
        data = new MovementOperationData
        {
            operationId = nextId,
            type = type,
            sourceId = sourceId,
            destinationId = destinationId,
            shifting = shifting
        };
        operations.Add(this);
        nextId++;
    }

    public void ProcessMove()
    {
        OperationNetworker.instance.SendMovementOperationRpc(data);
    }

    public static void ProcessResult(long id, OperationResult result)
    {
        MovementOperation operation = null;
        foreach(MovementOperation tempOp in operations)
        {
            if (tempOp.data.operationId == id)
                operation = tempOp;
        }

        if (operation?.data==null)
            return;

        switch (result)
        {
            case OperationResult.Success:
                operation.OnOperationSuccess?.Invoke();
                break;
            case OperationResult.Cancelled:
                operation.OnOperationCanceled?.Invoke();
                break;
            case OperationResult.Failure:
                operation.OnOperationFail?.Invoke();
                break;
        }
    }
}
