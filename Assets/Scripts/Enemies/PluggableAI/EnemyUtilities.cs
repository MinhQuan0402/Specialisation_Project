using UnityEngine;

public static class EnemyUtilities
{
    public static bool PlayerInRange(EnemyController controller, float range) =>
        controller.player && Vector2.Distance(controller.transform.position, controller.player.position) <= range;

    public static bool PlayerInSight(EnemyController controller, float range, LayerMask obstacleMask)
    {
        if (!controller.player) return false;

        Vector2 dir = new(controller.Movement.FacingDirection, 0f);
        RaycastHit2D hitPlayer = Physics2D.Raycast(controller.transform.position, dir, range, LayerMask.GetMask("Player"));
        bool playerHit = hitPlayer.collider != null;
        if (!playerHit) return false;

        RaycastHit2D hitObstacle = Physics2D.Raycast(controller.transform.position, dir, hitPlayer.distance, obstacleMask);
        bool obstacleHit = hitObstacle.collider != null;

        bool playerInSight = !obstacleHit && playerHit;
        if (playerInSight) controller.lastSeenPlayerPoint = controller.player.position;  // update last seen player position
        return playerInSight;  // no obstacles in the way
    }

    public static float DistanceToPlayer(EnemyController controller) =>
        controller.player ? Vector2.Distance(controller.transform.position, controller.player.position) : float.MaxValue;

    public static bool IsFacingPlayer(EnemyController controller)
    {
        if (!controller.player) return false;
        Vector2 dir = new(controller.player.position.x - controller.transform.position.x, 0.0f);
        return Mathf.Sign(dir.normalized.x) == controller.Movement.FacingDirection;
    }

    public static bool IsFacingAtPoint(EnemyController controller, Vector2 point)
    {
        Vector2 dir = new(point.x - controller.transform.position.x, 0.0f);
        return Mathf.Sign(dir.normalized.x) == controller.Movement.FacingDirection;
    }

    public static void FacePlayer(EnemyController controller)
    {
        if (!controller.player) return;
        Vector2 dir = new(controller.player.position.x - controller.transform.position.x, 0.0f);
        if (dir.normalized.x != controller.Movement.FacingDirection)
            controller.Movement.Flip();
    }
}
