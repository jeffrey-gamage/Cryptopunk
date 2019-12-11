using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breach : MonoBehaviour
{
    internal Program hacker;
    internal Hackable target;
    internal int power;
    internal List<DungeonTile> movePath;
    private DungeonTile myTile;
    [SerializeField] float animationSpeed = 3f;

    // Update is called once per frame
    private void Start()
    {
        DungeonManager.instance.Wait();
    }
    void Update()
    {
        AnimateMovement();
    }
    private void AnimateMovement()
    {
        Vector3 motion = (myTile.GetOccupyingCoordinates(false) - gameObject.transform.position).normalized * animationSpeed * Time.deltaTime;
        if (motion == Vector3.zero || motion.magnitude > (myTile.GetOccupyingCoordinates(false) - gameObject.transform.position).magnitude)
        {
            gameObject.transform.position = myTile.GetOccupyingCoordinates(false);
            if (movePath.Count == 0)
            {
                target.Breach(power);
                DungeonManager.instance.mode = DungeonManager.Mode.Move;
                Destroy(gameObject);
            }
            else
            {
                myTile = movePath[0];
                movePath.Remove(myTile);
            }
        }
        else
        {
            gameObject.transform.position += motion;
            gameObject.transform.localScale = new Vector3(Mathf.Clamp(motion.x, 0.1f, 0.6f), Mathf.Clamp(motion.y, 0.1f, 0.6f), 0.1f);
        }
    }
    internal void SetCourse(List<DungeonTile> path, Hackable target, Program hacker)
    {
        myTile = path[0];
        this.target = target;
        movePath = path;
    }
}
