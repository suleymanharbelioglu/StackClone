using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left,Right,Front,Back
}


public class PiceController : MonoBehaviour
{
    [SerializeField] private Transform reference;
    [SerializeField] private MeshRenderer referenceMesh;
    [SerializeField] private GameObject fallingPrefab;
    [SerializeField] private GameObject standPrefab;
    [SerializeField] private Transform last;
   
    [SerializeField][Range(1,5)] private float speed;
    [SerializeField][Range(1,2)] private float limt;

    private bool _isForward;
    private bool _isAxisX;
    private bool _isStop;

    

   

    private void LateUpdate()
    {
        if(_isStop) return;
        var position = transform.position;
        var direction = _isForward ? 1 : -1;
        
        var move = speed * Time.deltaTime * direction;

        if(_isAxisX)
        {
            position.x += move;
        }
        else
        {
            position.z += move;
        }
        // limt and turn
        if(_isAxisX)
        {
            if(position.x < -limt || position.x > limt)
            {
                position.x = Mathf.Clamp(position.x, -limt, limt);
                _isForward = !_isForward;
            }
        }
        else
        {
            if(position.z < -limt || position.z > limt)
            {
                position.z = Mathf.Clamp(position.z, -limt, limt);
                _isForward = !_isForward;
            }

        }
        transform.position = position;
    }

    private void DivideObject(bool isXaxis, float value)
    {
        
        bool isFirstFalling = value>0;

        var falling = Instantiate(fallingPrefab).transform;
        var stand  = Instantiate(standPrefab).transform;
        //size
        var fallingSize = reference.localScale;
        if(isXaxis)
        {
            fallingSize.x = Mathf.Abs(value) ;
        }
        else
        {
            fallingSize.z = Mathf.Abs(value) ;
        }
        
        falling.localScale = fallingSize;

        var standSize = reference.localScale;
        if(isXaxis)
        {  
            standSize.x = reference.localScale.x - Mathf.Abs(value);
        }
        else
        {
            standSize.z = reference.localScale.z - Mathf.Abs(value);
        }
        
        stand.localScale = standSize;

        var minDirection = isXaxis ? Direction.Left : Direction.Back;
        var maxDirection = isXaxis ? Direction.Right : Direction.Front;

        //Position
        var fallingPosition = GetPositionEdge(referenceMesh, isFirstFalling ? minDirection : maxDirection);
        if(isXaxis)
        {
            fallingPosition.x += (fallingSize.x /2) * (isFirstFalling ? 1 : -1);
        }
        else
        {
            fallingPosition.z += (fallingSize.z /2) * (isFirstFalling ? 1 : -1);
        }
        
        falling.position = fallingPosition;

        var standPosition = GetPositionEdge(referenceMesh, !isFirstFalling ? minDirection : maxDirection);
        if(isXaxis)
        {
            standPosition.x += (standSize.x /2) * (!isFirstFalling ? 1 : -1);
        }
        else
        {
            standPosition.z += (standSize.z /2) * (!isFirstFalling ? 1 : -1);
        }
        
        stand.position = standPosition;

        last  = stand;

        
    }

    private Vector3 GetPositionEdge(MeshRenderer mesh, Direction direction)
    {
        var extents = mesh.bounds.extents;
        var position = mesh.transform.position;

        switch(direction)
        {
            case Direction.Left:
            position.x += -extents.x;

            break;
            case Direction.Right:
            position.x += extents.x;
            break;
            case Direction.Front:
            position.z += extents.z;
            break;
            case Direction.Back:
            position.z += -extents.z;
            break;

        }
        return position;
    }

    public void OnClick()
    {
        _isStop = true;
        var distance = last.position - transform.position; 
        DivideObject(_isAxisX, _isAxisX ? distance.x : distance.z);

        //Reset
        _isAxisX = !_isAxisX;

        var newPosition = last.position;
        newPosition.y += transform.localScale.y;
        if(!_isAxisX) newPosition.z = limt;
        else newPosition.x = limt;
        transform.position = newPosition;


        transform.localScale = last.localScale;
        _isStop = false;
    }
    
    
}
