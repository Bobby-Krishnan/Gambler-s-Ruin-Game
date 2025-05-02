using UnityEngine;

public class GameResetManager : MonoBehaviour
{
    public Transform allZoneEnemies;
    public Transform allZoneGates;

    public void ResetAll()
    {
        foreach (Transform enemy in allZoneEnemies)
        {
            IResettable resettable = enemy.GetComponent<IResettable>();
            if (resettable != null)
                resettable.ResetObject();
        }

        foreach (Transform gate in allZoneGates)
        {
            IResettable resettable = gate.GetComponent<IResettable>();
            if (resettable != null)
                resettable.ResetObject();
        }
    }
}
