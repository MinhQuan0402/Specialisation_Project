using UnityEngine;

public static class EnemyUtilities
{
    public static bool PlayerInRange(EnemyController controller, float range) =>
        controller.player && Vector2.Distance(controller.transform.position, controller.player.position) <= range;

    public static float DistanceToPlayer(EnemyController controller) =>
        controller.player ? Vector2.Distance(controller.transform.position, controller.player.position) : float.MaxValue;

    public static void FacePlayer(EnemyController controller)
    {
        if (!controller.player) return;
        float dir = controller.player.position.x - controller.transform.position.x;
        if (Mathf.Abs(dir) > 0.01f)
            controller.transform.localScale = new Vector3(Mathf.Sign(dir), 1f, 1f);
    }
}
