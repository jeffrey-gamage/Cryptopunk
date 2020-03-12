using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    internal List<DungeonTile> movePath;
    protected DungeonTile myTile;
    [SerializeField] float animationSpeed = 3f;
    [SerializeField] GameObject attackTrail;

    // Update is called once per frame
    private void Start()
    {
        DungeonManager.instance.Wait();
    }
    void Update()
    {
        AnimateMovement();
        Instantiate(attackTrail, gameObject.transform.position, Quaternion.identity);
    }
    private void AnimateMovement()
    {
        Vector3 motion = (myTile.GetOccupyingCoordinates(false, false) - gameObject.transform.position).normalized * animationSpeed * Time.deltaTime;
        if (motion == Vector3.zero || motion.magnitude > (myTile.GetOccupyingCoordinates(false, false) - gameObject.transform.position).magnitude)
        {
            gameObject.transform.position = myTile.GetOccupyingCoordinates(false, false);
            if (movePath.Count == 0)
            {
                HitTarget();
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
    abstract protected void HitTarget();
}
