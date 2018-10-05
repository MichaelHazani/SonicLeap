using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class SonicController : MonoBehaviour
{

    public float MAX_SPEED = 4f;
    public int JUMP_FORCE = 400;

    public int MAX_RINGS_EMITTED = 32;

    [SerializeField]
    private bool __StartInPlacementMode;

    [SerializeField]
    private GameObject Ring;
    private Animator _anim;
    private Rigidbody _rb;
    private Camera _cam;
    private MLInputController _controller;
    private int _idleCnt;
    private bool _facingRight = true;
    private bool _sonicPlacementMode;
    private bool _isGrounded = true;
    private bool _isHurt = false;
    private bool _isRolling = false;
    private bool _isDucking = false;
    private MLResult _inputResult;
    private bool _playedSonicSoundOnce = false;
    private PhysicMaterial _sonicMaterial;
    private AudioSource _sonicAudioPlayer;

    [SerializeField]
    private AudioClip SpinDash;

    [SerializeField]
    private AudioClip SonicJump;

    [SerializeField]
    private AudioClip RingLoss;


    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _sonicAudioPlayer = GetComponent<AudioSource>();
        _sonicMaterial = GetComponent<Collider>().material;
        _cam = Camera.main;

        _inputResult = MLInput.Start();
        if (_inputResult.IsOk)
        {
            _controller = MLInput.GetController(MLInput.Hand.Left);
            MLInput.OnControllerButtonDown += OnButtonDown;
            MLInput.OnControllerButtonUp += OnButtonUp;
        }
        _sonicPlacementMode = __StartInPlacementMode;
        UpdatePlacementMode();
    }

    void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    void OnButtonDown(byte ctrl_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper && _isGrounded)
        {
            Jump();
        }
        else if (button == MLInputControllerButton.HomeTap)
        {
            _sonicPlacementMode = !_sonicPlacementMode;
            UpdatePlacementMode();
        }
    }
    void OnButtonUp(byte ctrl_id, MLInputControllerButton button)
    {

    }

    void UpdatePlacementMode()
    {
        if (_sonicPlacementMode)
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = _cam.transform.forward + _cam.transform.forward * 1.5f;
            transform.LookAt(_cam.transform);

            transform.parent = _cam.gameObject.transform;
        }
        else
        {

            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.parent = null;
        }
    }

    void FixedUpdate()
    {


        float X = 0, Y = 0;

        // if controller
        if (_inputResult.IsOk)
        {
            if (_controller.TriggerValue > 0.5)
            {
                if (_rb.velocity.x != 0f || _rb.velocity.z != 0f)
                {
                    _isRolling = true;

                    if (!_playedSonicSoundOnce)
                    {
                        _playedSonicSoundOnce = true;
                        _sonicAudioPlayer.PlayOneShot(SpinDash, 0.75f);
                    }
                }
                else
                {
                    _playedSonicSoundOnce = false;
                    _isRolling = false;
                    _isDucking = true;
                }
            }
            else
            {
                if (_isGrounded) _isRolling = false;
                _isDucking = false;
                _playedSonicSoundOnce = false;
            }

            if (_controller.Touch1PosAndForce.z > 0.0f)
            {
                if (!_isRolling)
                {
                    X = _controller.Touch1PosAndForce.x;
                    Y = _controller.Touch1PosAndForce.y;
                    Vector3 forward = Vector3.Normalize(Vector3.ProjectOnPlane(_cam.transform.forward, Vector3.up));
                    Vector3 right = Vector3.Normalize(Vector3.ProjectOnPlane(_cam.transform.right, Vector3.up));

                    _rb.velocity = new Vector3(X * MAX_SPEED, _rb.velocity.y, Y * MAX_SPEED);
                }
                transform.rotation = Quaternion.LookRotation(Vector3.Cross(_rb.velocity.normalized, transform.up.normalized));
            }
        }

        else
        {
            // if keyboard
            if (!_isRolling && !_isDucking && !_isHurt)
            {
                X = Input.GetAxis("Horizontal");
                Y = Input.GetAxis("Vertical");
                _rb.velocity = new Vector3(X * MAX_SPEED, _rb.velocity.y, Y * MAX_SPEED);
            }

            if (_rb.velocity.x != 0 || _rb.velocity.z != 0) transform.rotation = Quaternion.LookRotation(Vector3.Cross(_rb.velocity.normalized, transform.up.normalized));
            if (Input.GetKey(KeyCode.Space) && _isGrounded)
            {
                Jump();
            }

            if (Input.GetKey(KeyCode.C))
            {
                if (_rb.velocity.x != 0 || _rb.velocity.z != 0)
                {
                    _isRolling = true;


                    if (!_playedSonicSoundOnce)
                    {
                        _playedSonicSoundOnce = true;
                        _sonicAudioPlayer.PlayOneShot(SpinDash, 0.75f);
                    }
                }
                else
                {
                    _playedSonicSoundOnce = false;
                    _isRolling = false;
                    _isDucking = true;
                }
            }
            else
            {
                if (_isGrounded) _isRolling = false;
                _isDucking = false;
                _playedSonicSoundOnce = false;
            }



            if (Input.GetKey(KeyCode.L))
            {
                LoseRings();
            }
        }

        //friction
        if (_isRolling)
        {
            _sonicMaterial.dynamicFriction = 0.03f;
            _sonicMaterial.staticFriction = 0.03f;
        }
        else
        {
            _sonicMaterial.dynamicFriction = 0.2f;
            _sonicMaterial.staticFriction = 0.2f;
        }

        _anim.SetFloat("Speed", Mathf.Abs(_rb.velocity.x) + Mathf.Abs(_rb.velocity.z));

        if (_rb.velocity.x == 0 && _rb.velocity.z == 0 && _isGrounded && !_isDucking)
        {
            _idleCnt += 1;
            if (_idleCnt >= 120)
            {
                _idleCnt = 0;
                _anim.SetBool("Waiting", true);
            }
        }
        else
        {
            if (_idleCnt != 0)
            {
                _idleCnt = 0;
                _anim.SetBool("Waiting", false);
            }
        }

        _anim.SetBool("Rolling", _isRolling);
        _anim.SetBool("Ducking", _isDucking);
    }

    void Flip()
    {
        _facingRight = !_facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void Jump()
    {
        if (_isGrounded)
        {
            _isRolling = true;
            _anim.SetBool("Rolling", true);
            _sonicAudioPlayer.PlayOneShot(SonicJump);
            _isGrounded = false;
            _rb.AddForce(Vector3.up * JUMP_FORCE, ForceMode.Force);
        }
    }

    public void LoseRings()
    {
        _sonicAudioPlayer.PlayOneShot(RingLoss, 0.75f);
        int _lostRings = SonicGameData.Instance.GetRingCount();
        if (_lostRings > MAX_RINGS_EMITTED) _lostRings = MAX_RINGS_EMITTED;

        int _lostRingCircle = _lostRings / 2;

        //emit two concentric ring circles per http://info.sonicretro.org/SPG:Ring_Loss
        for (int i = 0; i < _lostRingCircle; i++)
        {
            GameObject ringInstance = Instantiate(Ring, GetPointInCircle(.5f, Mathf.PI * 2 / _lostRingCircle * i, transform.position), Quaternion.identity);
            ringInstance.GetComponent<Rigidbody>().AddExplosionForce(2f, transform.position, 10f, 3f, ForceMode.Impulse);
            StartCoroutine(ringInstance.GetComponent<Ring>().StartCountdownToDestruction());
        }
        for (int i = 0; i < _lostRingCircle; i++)
        {
            GameObject ringInstance = Instantiate(Ring, GetPointInCircle(.5f, Mathf.PI * 2 / _lostRingCircle * i, transform.position), Quaternion.identity);
            ringInstance.GetComponent<Rigidbody>().AddExplosionForce(4f, transform.position, 10f, 2f, ForceMode.Impulse);
            StartCoroutine(ringInstance.GetComponent<Ring>().StartCountdownToDestruction());
        }
        SonicGameData.Instance.ResetRingCount();
    }



    private Vector3 GetPointInCircle(float radius, float rad, Vector3 origin)
    {
        Vector3 point = new Vector3();
        point.x = radius * Mathf.Cos(rad) + origin.x;
        point.z = radius * Mathf.Sin(rad) + origin.z;
        return point;
    }

    void HurtSonic()
    {
        _isHurt = true;
        LoseRings();
        _anim.SetBool("Hurt", true);
    }

    void Ricochet(Vector3 otherPos)
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        Vector3 dir = otherPos - transform.position;
        dir.y -= 3f;
        dir = -dir.normalized;
        _rb.AddForce(dir * 150);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            if (!_isRolling)
            {
                HurtSonic();
            }
            else // kill enemy
            {
                col.gameObject.GetComponent<SonicEnemyBehavior>().SelfDestroy();
            }
            Ricochet(col.gameObject.transform.position);
        }
        else // col w/ground
        {
            _isHurt = false;
            _anim.SetBool("Hurt", false);
        }
    }
    void OnCollisionStay(Collision col)
    {
        _isGrounded = true;
        // _anim.SetBool("Hurt", false);
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag != "enemy")
        {
            _isGrounded = false;
            _anim.SetBool("Rolling", true);
        }
    }
}
