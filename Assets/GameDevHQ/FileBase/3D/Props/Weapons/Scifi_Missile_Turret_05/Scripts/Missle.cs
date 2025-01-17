﻿using GameDevHQ.Enemy.EnemyClassNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle
{
    [RequireComponent(typeof(Rigidbody))] //require rigidbody
    [RequireComponent(typeof(AudioSource))] //require audiosource
    public class Missle : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _particle = null; //reference to the particle system
        [SerializeField]
        private GameObject _explosionPrefab = null; //Explosion prefab to play on impact
        [SerializeField]
        private float _launchSpeed = 0f; //launch speed of the rocket
        [SerializeField]
        private float _power = 0f; //power of the rocket
        [SerializeField] //fuse delay of the rocket
        private float _fuseDelay = 0f;
        [SerializeField]
        private int _damage = 0;

        private Rigidbody _rigidbody; //reference to the rigidbody of the rocket
        private AudioSource _audioSource; //reference to the audiosource of the rocket

        private bool _launched = false; //bool for if the rocket has launched
        private float _initialLaunchTime = 2.0f; //initial launch time for the rocket
        private bool _thrust; //bool to enable the rocket thrusters

        private bool _fuseOut = false; //bool for if the rocket fuse
        private bool _trackRotation = false; //bool to track rotation of the rocket

        private Missle_Launcher.MissileType _missileType;
        private Transform _target;

        // Use this for initialization
        IEnumerator Start()
        {
            _rigidbody = GetComponent<Rigidbody>(); //assign the rigidbody component 
            _audioSource = GetComponent<AudioSource>(); //assign the audiosource component
            _audioSource.pitch = Random.Range(0.7f, 1.9f); //randomize the pitch of the rocket audio
            _particle.Play(); //play the particles of the rocket
            _audioSource.Play(); //play the rocket sound

            yield return new WaitForSeconds(_fuseDelay); //wait for the fuse delay

            _initialLaunchTime = Time.time + 1.0f; //set the initial launch time
            _fuseOut = true; //set fuseOut to true
            _launched = true; //set the launch bool to true 
            _thrust = false; //set thrust bool to false

        }


        // Update is called once per frame
        void Update()
        {
            if (_fuseOut == false) //check if fuseOut is false
                return;

            if (_launched == true) //check if launched is true
            {
                _rigidbody.AddForce(transform.forward * _launchSpeed); //add force to the rocket in the forward direction

                if (Time.time > _initialLaunchTime + _fuseDelay) //check if the initial launch + fuse delay has passed
                {
                    _launched = false; //launched bool goes false
                    _thrust = true; //thrust bool goes true
                }
            }

            if (_thrust == true) //if thrust is true
            {
                _rigidbody.useGravity = true; //enable gravity 
                _rigidbody.velocity = transform.forward * _power; //set velocity multiplied by the power variable
                _thrust = false; //set thrust bool to false
                _trackRotation = true; //track rotation bool set to true
            }

            if (_trackRotation == true) //check track rotation bool
            {
                if (_missileType == Missle_Launcher.MissileType.Normal) //checking for normal missile 
                {
                    _rigidbody.rotation = Quaternion.LookRotation(_rigidbody.velocity); // adjust rotation of rocket based on velocity
                    _rigidbody.AddForce(transform.forward * 100f); //add force to the rocket
                }
                else if (_missileType == Missle_Launcher.MissileType.Homing) //if missle is homing
                {
                    if (_target == null) //checking if the target is null
                    {
                        _missileType = Missle_Launcher.MissileType.Normal; //assign back to normal
                        return;
                    }

                    Vector3 direction = _target.position - transform.position; //calculate direciton for rocket to face
                    direction.Normalize(); //set the magnitude of the vector to 1
                    Vector3 turnAmount = Vector3.Cross(transform.forward, direction); //using cross product, we multiply our forward vector of the rocket by the direction vector, to create a perpendular vector, which specifies the turn amount

                    _rigidbody.angularVelocity = turnAmount * _power; //apply angular velocity
                    _rigidbody.velocity = transform.forward * _power; //apply forward velocity

                }
            }
        }

        /// <summary>
        /// This method is used to assign traits to our missle assigned from the launcher.
        /// </summary>
        public void AssignMissleRules(Missle_Launcher.MissileType missileType, Transform target, float launchSpeed, float power, float fuseDelay, float destroyTimer, int damage)
        {
            _missileType = missileType; //assign the missle type
            _target = target; //Who should the rocket follow?
            _launchSpeed = launchSpeed; //set the launch speed
            _power = power; //set the power
            _fuseDelay = fuseDelay; //set the fuse delay
            _damage = damage;
            Destroy(this.gameObject, destroyTimer); //destroy the rocket after destroyTimer 
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Enemy"))
            {

                other.collider.GetComponent<EnemyClass>().Damage(_damage);
            }

            if (_explosionPrefab != null)
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity); //instantiate explosion

            Destroy(this.gameObject); //destroy the rocket (this)
        }
    }
}

