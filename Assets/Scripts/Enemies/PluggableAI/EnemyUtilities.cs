using UnityEngine;

public static class EnemyUtilities
{
    public static bool PlayerInRange(EnemyController controller, float range) =>
        controller.player && Vector2.Distance(controller.transform.position, controller.player.position) <= range;

    public static bool PlayerInSight(EnemyController controller, float range, LayerMask detectibleMasks)
    {
        if (!controller.player) return false;

        Vector2 directionToPlayer = controller.player.position - controller.transform.position;
        if (directionToPlayer.sqrMagnitude > range * range) return false;

        Vector2 normDirToPlayer = directionToPlayer.normalized;
        Vector2 facingDirection = new(controller.Movement.FacingDirection, 0f);
        if (controller.Data.isFlying)
        {
            float dot = Vector2.Dot(normDirToPlayer, facingDirection);
            float halfAngle = controller.Data.playerDetectionAngle * 0.5f;

            float cosLimit = Mathf.Cos(halfAngle * Mathf.PI / 180.0f);
            if (dot < cosLimit) return false;

            facingDirection = normDirToPlayer;
        }


        RaycastHit2D hit = Physics2D.Raycast(controller.transform.position, facingDirection, 
                                                   range, detectibleMasks);
        bool isPlayerHit = (hit.collider != null) && 
                           (hit.collider.gameObject.layer == controller.player.gameObject.layer);
        if (isPlayerHit) controller.lastSeenPlayerPoint = hit.point;
        return isPlayerHit;
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
