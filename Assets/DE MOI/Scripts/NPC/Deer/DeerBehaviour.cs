using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GENERAL;

public class DeerBehaviour : MonoBehaviour
{
    public static float distanceDespown = 40;
    public float runSpeed = 6;
    public float life = 100;
    public Transform ragdollPrefab;
    public Transform armature;
    public float distanceToPlayer;
    public PickabelObject pickabelAfterDeath;

    protected Animator animator;
    private Rigidbody rb;

    private float walking = 0;
    private float walkingTarget = 0;
    private float alarmat = 0;
    private NPC_state state = NPC_state.IDLE;
    private NPC_state lastState = NPC_state.IDLE;
    private float angle = 0;
    private Vector3 originalRotation;

    private bool isItHurt = false;

    private Vector3 lastPosition;
    protected SoundManager soundManager;
    //private float ammountOfSnow = 0;

    // Start is called before the first frame update
    protected virtual void initializ()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        originalRotation = transform.localRotation.eulerAngles;
        lastPosition = transform.position;
        soundManager = GetComponent<SoundManager>();
    }
    protected void Start()
    {
        initializ();
        soundManager.Play("idle", true);
    }
    protected virtual void kill()
    {
        PickabelObjectScript pos = gameObject.AddComponent<PickabelObjectScript>();
        pos.pickData = pickabelAfterDeath;
        CopyArmatureComponents.copyData(armature, ragdollPrefab);
        Destroy(animator);
        Destroy(rb);
        Destroy(this);
    }
    public void getHit(float damage)
    {
        Debug.Log("Got hit");
        soundManager.Play("hit", true);
        alarmat = 5;
        state = NPC_state.RunningAway;
        life -= damage;
        if (life <= 0)
        {
            kill();
        }
        isItHurt = true;
    }
    private float lastMoan = 0;
    private void idle()
    {
        walkingTarget = 0;
        if (Random.value > 0.999)
        {
            state = NPC_state.Searching;
        }
        if (lastState != NPC_state.IDLE)
            soundManager.Pause("walk");
        lastState = state;
        if(lastMoan > 5)
        {
            soundManager.Play("idle", true);
            lastMoan = 0;
        }
        else
        {
            lastMoan += Time.deltaTime * Random.value;
        }
    }

    private void move(float speed = 1, bool isItBleeding = false)
    {
        float snowHeight = 0;
        if (TerrainGenerator.instance)
        {
            snowHeight = TerrainGenerator.instance.getShowHeight(transform.position) * 4;
            if (Vector3.Distance(transform.position, lastPosition) > 0.3f)
            {
                TerrainGenerator.instance.walkedOver(transform.position, Time.deltaTime * speed * snowHeight, true);
                if (isItBleeding)
                    TerrainGenerator.instance.bleedOver(transform.position);
                lastPosition = transform.position;
            }
            else
            {
                TerrainGenerator.instance.walkedOver(transform.position, Time.deltaTime * speed * snowHeight, false);
            }
            walkingTarget -= Mathf.Sqrt(snowHeight) * speed * 25;
            if (walkingTarget < 0)
                walkingTarget = 0;
        }
        float angle = transform.rotation.eulerAngles.y * Mathf.PI / 180f;
        Vector3 right = new Vector3(Mathf.Cos(angle), 0, -Mathf.Sin(angle));
        Vector3 fowrord = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        transform.localPosition += fowrord * walking * Time.deltaTime;
    }

    private void rotate(float howMuch = 1)
    {
        this.angle += Time.deltaTime * (0.5f - Random.value) * howMuch;
        this.angle = Mathf.Lerp(this.angle, 0, Time.deltaTime);
        originalRotation.y += this.angle;
        transform.localRotation = Quaternion.Euler(originalRotation);
    }

    private void searching()
    {
        walkingTarget = runSpeed / 3;
        move(2);
        rotate(5);
        if (Random.value > 0.999)
        {
            state = NPC_state.IDLE;
        }
        if (lastState == NPC_state.IDLE)
        {
            Debug.Log("Unposing walk");
            soundManager.UnPause("walk");
        }
        lastState = state;
    }

    private void running()
    {
        walkingTarget = runSpeed;
        move(5, isItHurt);
        rotate(30);
        if (lastState == NPC_state.IDLE)
        {
            Debug.Log("Unposing walk");
            soundManager.UnPause("walk");
        }
        lastState = state;
    }

    // Update is called once per frame
    void Update()
    {
        walking = Mathf.Lerp(walking, walkingTarget, Time.deltaTime * 5);
        animator.SetFloat("WalkingSoeed", walking);

        if(alarmat > 0)
        {
            alarmat -= Time.deltaTime;
            if (alarmat < 0)
            {
                state = NPC_state.Searching;
                isItHurt = false;
            }
        }

        if (PlayerController.instance)
        {
            float aux = Vector3.Distance(transform.localPosition, PlayerController.instance.transform.localPosition);
            if (aux < distanceToPlayer)
                state = NPC_state.RunningAway;
            else if (aux >= distanceDespown)
                Destroy(gameObject);
        }


        if (state == NPC_state.IDLE)
            idle();
        else if (state == NPC_state.Searching)
            searching();
        else running();
    }

}
