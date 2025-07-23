using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Moveset", menuName = "Moveset", order = 1)]
[System.Serializable]
public class Moveset : ScriptableObject
{
    public Move[] moves;

    [Header("AI")]
    public string[] closeGroundOptions;
    public string[] mediumGroundOptions;
    public string[] longGroundOptions;
    public string[] farGroundOptions;
    public string[] closeAirOptions;
    public string[] mediumAirOptions;
    public string[] longAirOptions;
    public string[] farAirOptions;
    public string[] antiAirOptions;
    public string[] antiGroundOptions;
    public string[] followupOptions;
    public string[] lowRecoveryOptions;
    public string[] highRecoveryOptions;
}

[System.Serializable]
public class Move
{
    public string name;
    public string animationName;
    public string input;
    public bool grounded = true;
    public bool airborne = true;
    public float superArmour;
    public string[] nextMoves;
    public HitboxInfo[] hitboxes;
}

[System.Serializable]
public class HitboxInfo
{
    public float damage = 1;
    public float hitstun = 0.2f;
    public float hitstop = 0.05f;
    public float attraction = 0f;
    public float attackerHitstopMultiplier = 1;
    public float scaledKnockback = 2.5f;
    public float unscaledKnockback = 0f;
    public float angle = 0;
    public bool guaranteeLaunch = false;
    public bool forwardDependentAngle = false;
    public bool directionIndependentAngle = false;
    public HitboxType type;
    public GameObject hitEffect;
}

public enum HitboxType
{
    Normal, Low, Overhead, Unblockable
}