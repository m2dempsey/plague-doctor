using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour {
    public float kMoveSpeed = 2f;
    public float kChanceToInfect = 0.999f;
    public float kChanceToInfectWithMask = 0.2f;
    public float kChanceToInfectWithSocialDistancing = 0.1f;
    public Vector2 kSleepTimeRange = new Vector2(1, 5);
    public Vector2 kWalkTimeRange = new Vector2(1, 5);

    public Vector2 kIncubatingTimeRange = new Vector2(1, 5);
    public Vector2 kAsymptomaticTimeRange = new Vector2(1, 5);
    public float kChanceWillShowSymptoms = 0.8f;
    public Vector2 kSyptomaticTimeRange = new Vector2(20, 30);
    public float kChanceWillGoCritical = 0.4f;
    public Vector2 kCriticaltimeRange = new Vector2(1, 10);
    public float kChanceWillDie = 0.3f;
    public float kChanceWillDieWithoutSupport = 0.8f;

    public int kNumCriticalSupported = 5;


    public Sprite kHealthySprite;
    public Sprite kSickSprite;
    public Sprite kCriticalSprite;
    public Sprite kRecoveredSprite;

    public AudioClip kCriticalSound;
    public AudioClip kDeathSound;
    public AudioClip kInfectedSound;
    public AudioClip kPositiveTestSound;

    public GameObject mask;
    public GameObject socialDistance;
    private bool wearingMask = false;
    private bool socialDistancing = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource hitAudioSource;
    private AudioSource criticalAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource infectedAudioSource;
    private AudioSource positiveTestAudioSource;

    enum ActionState {
        Waiting,
        Sleeping,
        Walking
    }
    private ActionState actionState = ActionState.Waiting;
    enum InfectionState {
        Healthy,
        InitialInfection,
        Incubating,
        Asymptomatic,
        Symptomatic,
        Critical,
        Recovered
    }  
    private InfectionState infectionState = InfectionState.Healthy; 

    public static int numCritical = 0;
    public static int numInfected = 0;
    public static int numHumans = 0;
    public static int numDeaths = 0;

    void Start() {
        rb = this.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        criticalAudioSource = AddAudio(kCriticalSound);
        deathAudioSource = AddAudio(kDeathSound);
        infectedAudioSource = AddAudio(kInfectedSound);
        positiveTestAudioSource = AddAudio(kPositiveTestSound);
        numHumans++;
    }

    public void Infect() {
        if (infectionState == InfectionState.Healthy) {
            infectionState = InfectionState.InitialInfection;
            numInfected++;
        }
    }

    private AudioSource AddAudio(AudioClip audioClip) {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = audioClip;
        newAudio.loop = false;
        newAudio.playOnAwake = false;
        newAudio.volume = 1f;
        return newAudio;
    }

    IEnumerator ProcessActionState() {
        float sleepTime = Random.Range(kSleepTimeRange.x, kSleepTimeRange.y);
        float walkTime = Random.Range(kWalkTimeRange.x, kWalkTimeRange.y);
              
        if (infectionState != InfectionState.Critical && !socialDistancing) {
            actionState = ActionState.Walking;
            float angle = Random.Range(0, 2 * Mathf.PI);
            rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * kMoveSpeed;
            yield return new WaitForSeconds(walkTime);
        }

        actionState = ActionState.Sleeping;
        rb.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(sleepTime);

        actionState = ActionState.Waiting;
    }

    IEnumerator ProcessInfectionState() {
        float incubatingTime = Random.Range(kIncubatingTimeRange.x, kIncubatingTimeRange.y);
        float asymptomaticTime = Random.Range(kAsymptomaticTimeRange.x, kAsymptomaticTimeRange.y);
        bool willShowSymptoms = Random.value < kChanceWillShowSymptoms;
        float symptomaticTime = Random.Range(kSyptomaticTimeRange.x, kSyptomaticTimeRange.y);
        bool willGoCritical = Random.value < kChanceWillGoCritical;
        float criticalTime = Random.Range(kCriticaltimeRange.x, kCriticaltimeRange.y);
        bool willDie = Random.value < kChanceWillDie;
        bool willDieWithoutSupport = Random.value < kChanceWillDieWithoutSupport;

        infectionState = InfectionState.Incubating;
        yield return new WaitForSeconds(incubatingTime);

        infectionState = InfectionState.Asymptomatic;
        yield return new WaitForSeconds(asymptomaticTime);

        if (willShowSymptoms) {
            infectionState = InfectionState.Symptomatic;
            spriteRenderer.sprite = kSickSprite;
            infectedAudioSource.Play();
            yield return new WaitForSeconds(symptomaticTime);
        
            if (willGoCritical) {
                numCritical++; 
                infectionState = InfectionState.Critical;
                spriteRenderer.sprite = kCriticalSprite;
                criticalAudioSource.Play();
                rb.velocity = new Vector2(0, 0);
                yield return new WaitForSeconds(criticalTime);
                numCritical--;

                if (numCritical < kNumCriticalSupported ? willDie : willDieWithoutSupport) {
                    deathAudioSource.Play();
                    yield return new WaitForSeconds(0.3f);
                    
                    numDeaths++;
                    numHumans--;
                    Destroy(gameObject);
                }
            }
        }

        numInfected--;
        infectionState = InfectionState.Recovered;
        spriteRenderer.sprite = kRecoveredSprite;
    }

    void Update() {
        if (actionState == ActionState.Waiting) {
            StartCoroutine(ProcessActionState());
        }
        if (infectionState == InfectionState.InitialInfection) {
            StartCoroutine(ProcessInfectionState());
        }
    }

    void OnCollisionEnter2D (Collision2D collision) {
        if (collision.gameObject.tag == "Human" &&
         (infectionState == InfectionState.Symptomatic || infectionState == InfectionState.Asymptomatic || infectionState == InfectionState.Critical) &&
         Random.value < (wearingMask ? kChanceToInfectWithMask : kChanceToInfect)) {
            HumanController other_hc = collision.gameObject.GetComponent<HumanController>();
            if (socialDistancing && other_hc.socialDistancing) {
                return;
            }
            if (socialDistancing || other_hc.socialDistancing) {
                if (Random.value < kChanceToInfectWithSocialDistancing) {
                    other_hc.Infect();
                }
            } else {
                other_hc.Infect();
            }
        }
    }

    public void WearMask() {
        mask.GetComponent<SpriteRenderer>().enabled = true;
        wearingMask = true;
    }

    public void Test() {
        if (infectionState == InfectionState.Asymptomatic && spriteRenderer.sprite != kSickSprite) {
            positiveTestAudioSource.Play();
            spriteRenderer.sprite = kSickSprite;
        }
    }

    public void ApplySocialDistancing() {
        socialDistance.GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CircleCollider2D>().radius = 0.32f; // hardcoded
        socialDistancing = true;
    }
}
