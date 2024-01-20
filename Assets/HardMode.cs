using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardMode : MonoBehaviour
{
    public static bool isHardMode;

    public void ToggleHardmode() => isHardMode = !isHardMode;
}
