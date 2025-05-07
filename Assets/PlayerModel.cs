using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerModel : MonoBehaviour
{
    public PlayerScript player;
    public Animator animator;
    public MoveID move;
    public Emotion emotion;
    public bool allowFollowup;
    public float targetRotation = 135;
    public bool crouching;
    public Vector2 velocity;
    public bool overrideXVelocity;
    public bool overrideYVelocity;
    public bool alignToVelocity;
    public bool invincible;
    public bool talking;
    public bool ledgeSnap;
    public MeshRenderer face;
    [Header("Emotions")]
    public Material idle;
    public Material angry;
    public Material angrier;
    public Material ohCrap;
    public Material damaged;
    public Material hurt;
    public Material wincing;

    public enum MoveID
    {
        None, Jab, DashAttack, ForwardTilt, BackTilt, UpTilt, DownTilt, NeutralAerial, ForwardAerial, BackAerial, UpAerial, DownAerial, ForwardStrong, BackStrong, UpStrong, DownStrong, NeutralSpecial, ForwardSpecial, BackSpecial, UpSpecial, DownSpecial, Grab, Pummel, UpThrow, DownThrow, BackThrow, ForwardThrow, Taunt, Extra1, Extra2, Extra3, Extra4, Extra5,
    }

    public enum Emotion
    {
        Idle, Angry, Angrier, Grin, Hurt, Wincing
    }

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<PlayerScript>();
        animator.keepAnimatorStateOnDisable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (!player.dodging)
        {
            animator.SetFloat("DodgeDirection", player.input.GetLeftStickX() * player.direction);
        }

        animator.SetFloat("VelocityX", (player.velocity.x / player.fighter.walkSpeed) * player.direction);
        animator.SetFloat("VelocityY", player.velocity.y);
        animator.SetFloat("InputX", player.input.GetLeftStickX() * player.direction);
        animator.SetFloat("InputY", player.input.GetLeftStickY());
        animator.SetFloat("Crouching", Mathf.Lerp(animator.GetFloat("Crouching"), (player.input.GetLeftStickY() < -0.25f ? 1 : 0), Time.deltaTime * 25));
        animator.SetFloat("Speed", Mathf.Max(1, Mathf.Abs(player.velocity.x)));
        animator.SetFloat("Unactionable", Mathf.Max(0, player.unactionable * Mathf.Infinity));
        animator.SetFloat("Talking", talking ? 0 : 1);
        animator.SetBool("Grounded", player.grounded);
        animator.SetBool("Blocking", player.blockStop);
        animator.SetBool("Hitstun", player.hitstun > 0 && !player.free);
        animator.SetBool("AttackDown", player.input.GetAttack());
        animator.SetBool("SpecialDown", player.input.GetSpecial());

        animator.speed = player.hitstop <= 0 ? 1 : 0;
    }

    void LateUpdate()
    {
        if (player.hitstun > 0)
        {
            //transform.position += new Vector3(Random.Range(player.hitstun * -1, player.hitstun), Random.Range(player.hitstun * -1, player.hitstun), 0);
        }
    }

    public void ShakeCamera(float amount)
    {
        player.multiplayer.cameraScript.ShakeCamera(Vector2.one * amount);
    }

    public void SetEmotion()
    {
        Material nextMaterial = idle;

        if (player.hitstun > 0 && player.hitstop > 0 && player.fighter.currentMove == null)
        {
            nextMaterial = ohCrap;
        }
        else if (player.hitstun > 0)
        {
            nextMaterial = damaged;
        }
        else if (emotion == Emotion.Idle)
        {
            nextMaterial = idle;
        }
        else if (emotion == Emotion.Angrier || emotion == Emotion.Angry)
        {
            if (emotion == Emotion.Angrier && angrier != null)
            {
                nextMaterial = angrier;
            }
            else if (angry != null)
            {
                nextMaterial = angry;
            }
        }
        else if (emotion == Emotion.Hurt)
        {
            nextMaterial = hurt;
        }
        else if (emotion == Emotion.Wincing)
        {
            nextMaterial = wincing;
        }

        if (nextMaterial != null && nextMaterial != face.material)
        {
            face.material = nextMaterial;
        }
    }

    public void SpawnProjectile(int ID)
    {
        player.fighter.projectileSpawners[ID].Fire(player);
        player.fighter.OnProjectileSpawn(ID);
    }
}
