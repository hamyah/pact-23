using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class PersonInteractableActivated : UnityEvent<int, int, Vector2>{}

[System.Serializable]
public class SoundProjectileExpired : UnityEvent{}

[System.Serializable]
public class ChangedProgress : UnityEvent<float>{}

[System.Serializable]
public class RequestPickupSpawn : UnityEvent<string>{}

public class Player : MonoBehaviour
{
    public static Vector3 personScale;

    public static PersonInteractableActivated m_PersonInteractableActivated;
    public static SoundProjectileExpired m_SoundProjectileExpired;
    public static ChangedProgress m_ChangedProgress;
    public static RequestPickupSpawn m_RequestPickupSpawn;

    [Header("Controls")]
    public float rotateSpeed;

    [Header("People")]
    public bool test;
    public float circleRadius;
    public int currentPeople;
    public int maxPeoplePerCircle = 10;
    public float distanceBetweenPeople = 1.25f;
    public GameObject personPrefab;
    public bool controlsEnabled = false;
    public bool firstSoundSpawned = false;

    private Transform peopleHolder;
    private GameObject firstPerson;

    [Header("Projectile")]
    public GameObject soundProjectile;
    public Transform soundSpawner;

    [Header("Pickups")]
    public GameObject personPickupPrefab;
    public float spawnRadiusPercentage;

    [Header("UI")]
    public GameObject tutorial;
    private GameObject tutorialInstance;

    public TextMeshProUGUI scoreText;
    public int score;

    void Start() {
        if (m_PersonInteractableActivated == null)
            m_PersonInteractableActivated = new PersonInteractableActivated();

        if (m_SoundProjectileExpired == null)
            m_SoundProjectileExpired = new SoundProjectileExpired();

        if (m_ChangedProgress == null)
            m_ChangedProgress = new ChangedProgress();

        if (m_RequestPickupSpawn == null)
            m_RequestPickupSpawn = new RequestPickupSpawn();

        m_PersonInteractableActivated.AddListener(AddPerson);

        m_SoundProjectileExpired.AddListener(WhenSoundProjectileExpired);

        m_RequestPickupSpawn.AddListener(RequestPickupSpawn);



        // Person scale is 1 when there are 20 people and radius 4
        personScale = new Vector3(20f/maxPeoplePerCircle*circleRadius/4f, 20f/maxPeoplePerCircle*circleRadius/4f, 1f);

        peopleHolder = transform.Find("PeopleHolder");

        score = 0;
        
        AddPerson();
    }

    void Update() {
        if(controlsEnabled) {
            transform.Rotate(new Vector3(0f, 0f, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime));

            if(!firstSoundSpawned) {
                if(Input.GetKeyDown(KeyCode.Space)) {
                    Destroy(tutorialInstance, 5f);
                    tutorialInstance.GetComponent<Animator>().Play("fade out");


                    SpawnSoundProjectile();
                }
            }
        }

        if(test) {
            test = false;
            AddPerson();
        }
    }

    void AddPerson(int numberOfPeople, int typeOfPerson, Vector2 pickupPos) {
        for (int i = 0; i < numberOfPeople; i++)
            AddPerson(typeOfPerson, pickupPos);
    }

    void AddPerson() {
        AddPerson(Random.Range(1, 5), new Vector2(0, 15f));
    }

    public void AddPerson(int typeOfPerson, Vector2 pickupPos) {
        if(currentPeople == maxPeoplePerCircle) return;

        GameObject newPerson = Instantiate(personPrefab, new Vector3(0f, -circleRadius, 0f), Quaternion.identity, peopleHolder);
        newPerson.transform.localScale = personScale;
        newPerson.transform.RotateAround(transform.position, new Vector3(0f, 0f, 1f), currentPeople*(360/maxPeoplePerCircle) + transform.eulerAngles.z);

        GameObject sprite = newPerson.transform.Find("Sprite").gameObject;
        sprite.transform.position = pickupPos;
        if(currentPeople == 0) {
            firstPerson = sprite;
            sprite.transform.eulerAngles = new Vector3(0, 0, 180);
            LeanTween.rotateLocal(sprite, new Vector3(0, 0, 200f), 0.2f).setRepeat(24).setLoopPingPong();
            LeanTween.move(sprite, newPerson.transform, 5f).setOnComplete(StartGame);

        } else {
            LeanTween.move(sprite, newPerson.transform, 0.5f);

        }

        Debug.Log("Alentejanos/alentejano" + typeOfPerson + ".png");
        sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Alentejanos/alentejano" + typeOfPerson);


        currentPeople++;
        UpdateScore(++score);

        m_ChangedProgress.Invoke((float)currentPeople/maxPeoplePerCircle);

        if(currentPeople == maxPeoplePerCircle) {
            // Round over
            Debug.Log("Round over");
        }
        
    }

    void RemovePerson() {
        if(currentPeople == 1) return;

        currentPeople--;
        UpdateScore(--score);
        Destroy(peopleHolder.GetChild(peopleHolder.childCount-1).gameObject);

        m_ChangedProgress.Invoke((float)currentPeople/maxPeoplePerCircle);
    }

    public void SpawnSoundProjectile() {
        /*float angle = currentPeople*(360f/maxPeoplePerCircle)/2f + transform.eulerAngles.z;
        //soundSpawner.localPosition = new Vector2(Mathf.Sin(angle*Mathf.Deg2Rad)*circleRadius, Mathf.Cos(angle*Mathf.Deg2Rad)*circleRadius);
        //soundSpawner.eulerAngles = new Vector3(0, 0, currentPeople/2*(360/maxPeoplePerCircle));

        Vector2 first = peopleHolder.GetChild(0).position;
        Vector2 last = peopleHolder.GetChild(peopleHolder.childCount-1).position;
        Vector2 midPos = Vector2.Lerp(first, last, 0.5f);
        float distPerc = Vector3.Distance(midPos, Vector2.zero);
        Vector2 pos = Vector2.Lerp(midPos, transform.position, 0.2f);
        Debug.Log(distPerc + " / " + circleRadius);
        soundSpawner.position = pos;*/
        Debug.Log("do it");
        GameObject projectile = Instantiate(soundProjectile, soundSpawner.position, soundSpawner.rotation);
    }

    void WhenSoundProjectileExpired() {
        RemovePerson();
        SpawnSoundProjectile();
    }

    void RequestPickupSpawn(string desc) {
        if(desc == "person") {
            SpawnPersonPickup();
        }
    }

    void SpawnPersonPickup() {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Mathf.Lerp(0, circleRadius*spawnRadiusPercentage, Random.Range(0f, 1f));

        GameObject newPickup = Instantiate(personPickupPrefab, new Vector2(Mathf.Cos(randomAngle)*randomDistance, Mathf.Sin(randomAngle)*randomDistance), Quaternion.identity);
        
    }

    void StartGame() {
        controlsEnabled = true;
        LeanTween.rotateLocal(firstPerson, Vector3.zero, 0.5f);

        // Spawn instructions
        tutorialInstance = Instantiate(tutorial);
    }

    void UpdateScore(int s) {
        scoreText.text = s.ToString();
    }
    
}
