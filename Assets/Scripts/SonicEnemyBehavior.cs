using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicEnemyBehavior : MonoBehaviour
{

    [SerializeField]
    private int MAX_SPEED;

    [SerializeField]
    private int ROT_SPEED;
    private bool _isTurning;

    AudioSource _enemyAudioSource;

    [SerializeField]
    AudioClip _killSound;

    Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _enemyAudioSource = GetComponent<AudioSource>();
    }

    IEnumerator TurnInRandomRot()
    {
        _isTurning = true;
        // _rb.isKinematic = true;
        Quaternion _currentRot = transform.rotation;
        float _rotDir = 90 * (Random.Range(0, 2) * 2 - 1);
        Quaternion _destRot = transform.rotation * Quaternion.Euler(0, _rotDir, 0);
        float rate = 1.0f * ROT_SPEED;
        float t = 0.0f;
        while (t < 1.0f)
        {
            _rb.MoveRotation(Quaternion.Slerp(_currentRot, _destRot, t));
            t += Time.fixedDeltaTime * rate;
            yield return new WaitForFixedUpdate();
        }
        _isTurning = false;
        // _rb.isKinematic = false;
    }

    public void SelfDestroy()
    {
        _enemyAudioSource.PlayOneShot(_killSound);
        gameObject.GetComponent<Renderer>().enabled = false;
        SonicGameData.Instance.RemoveEntity(gameObject);
    }
    void FixedUpdate()
    {
        if (!_isTurning)
        {
            Vector3 LookAheadDown = -transform.right + new Vector3(0, -.4f, 0);
            if (Debugger.Instance.DebugEnemies) Debug.DrawRay(transform.position, LookAheadDown, Color.blue);
            if (Debugger.Instance.DebugEnemies) Debug.DrawRay(transform.position, -transform.right, Color.red);

            RaycastHit hit;
            RaycastHit hit2;
            if (Physics.Raycast(transform.position, LookAheadDown, out hit, 1f) && !Physics.Raycast(transform.position, -transform.right, out hit2, 1f))
            {
                // print(gameObject.name + " sees floor and no obstacles!");
                _rb.AddForce(-transform.right * MAX_SPEED);
            }
            else
            {
                // print(gameObject.name + " doesn't see floor or sees obstacles, turning!");
                StartCoroutine(TurnInRandomRot());
            }
        }
    }
}
