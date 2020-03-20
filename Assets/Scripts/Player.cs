using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _runSpeed = 6.0f;
    [SerializeField]
    private float _gravity = -9.8f;
    [SerializeField]
    private float _jumpVelocity = 9.8f;
    // i hate this as a solution for ground detection. i want to
    // learn how to do proper Rigidbody-based movement soon, because
    // hacks like this shouldn't be necessary unless they make eminent sense.
    [SerializeField]
    private float _minY = -0.1f;
    [SerializeField]
    private bool _canDoubleJump = true;

    /*
      NOTE
      i'd love to see if i could put variables like this into
      scriptable objects, that would be really lovely for
      gameplay balancing
    */

    [SerializeField]
    private Vector3 _dir = Vector3.zero;
    [SerializeField]
    private Vector3 _mvmt = Vector3.zero;

    private CharacterController _controller;

    void Start()
    {
      _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
      // interesting problem:
      // Input.GetAxisRaw continues returning the last read value
      // when the game loses focus in the editor, not sure if the
      // same thing applies to a full build losing focus in the DE,
      // but i still wonder what can be done about it
      // NOTE: heap-allocating fresh vectors on each frame doesn't
      // solve the problem
      // NOTE: GetAxis vs GetAxisRaw doesn't seem to make a difference either
      float horizontalInput = Input.GetAxisRaw("Horizontal");

      // calculate basic components of movement direction
      _dir.x = horizontalInput;

      // adjust magnitude of directional components into another Vector3
      _mvmt.x = _dir.x * _runSpeed;

      // shmuck custom gravity
      if (_controller.isGrounded)
      {
        _canDoubleJump = true;
        // "ground pushing up"
        _mvmt.y = _minY;
        if (Input.GetKeyDown(KeyCode.Space))
        {
          _mvmt.y = _jumpVelocity;
        }
      }
      else if (Input.GetKeyDown(KeyCode.Space) && _canDoubleJump)
      {
        _canDoubleJump = false;
        _mvmt.y = _jumpVelocity; // cancels gravity, which may not be the desired behavior depending on the game
      }

      // always apply gravity
      _mvmt.y += _gravity * Time.deltaTime;

      // final movement frame-adjusted
      _controller.Move(_mvmt * Time.deltaTime);
    }
}
