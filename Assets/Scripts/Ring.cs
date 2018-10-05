using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{

    public float ROTATION_SPEED = -8f;

    private AudioSource _ringAudioSource;

    [SerializeField]
    private AudioClip _collectedRing;

    // Use this for initialization
    void Start()
    {
        _ringAudioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 1f * ROTATION_SPEED, 0f);

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Sonic")
        {
            StopAllCoroutines();
            _ringAudioSource.PlayOneShot(_collectedRing);
            gameObject.GetComponent<Renderer>().enabled = false;
            SonicGameData.Instance.IncreaseRingCount();
            Destroy(gameObject, _collectedRing.length);

        }
    }

    public IEnumerator StartCountdownToDestruction()
    {

        yield return new WaitForSeconds(7);
        if (this != null)
        {
            Destroy(gameObject);
        }
    }


}
