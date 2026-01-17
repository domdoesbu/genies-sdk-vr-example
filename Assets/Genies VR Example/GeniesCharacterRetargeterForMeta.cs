using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.Movement.Retargeting;
using Genies.Sdk;
using Oculus.Interaction.Locomotion;
using System;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace Genies.VRExample
{
    public class GeniesCharacterRetargeterForMeta : CharacterRetargeter
    {
        public override void Awake()
        {
            Transform controllerRoot = transform;

            Transform root = controllerRoot.FindChildRecursive("Root");
            Transform hips = controllerRoot.FindChildRecursive("Hips");
            //Transform hipsBind = controllerRoot.FindChildRecursive("HipsBind");
            Transform leftUpLeg = controllerRoot.FindChildRecursive("LeftUpLeg");
            Transform leftLeg = controllerRoot.FindChildRecursive("LeftLeg");
            Transform leftFoot = controllerRoot.FindChildRecursive("LeftFoot");
            Transform leftFootBind = controllerRoot.FindChildRecursive("LeftFootBind");
            Transform leftToeBase = controllerRoot.FindChildRecursive("LeftToeBase");
            // Transform leftToeBaseBind = controllerRoot.FindChildRecursive("LeftToeBaseBind");
            // Transform leftLegTwist1Bind = controllerRoot.FindChildRecursive("LeftLegTwist1Bind");
            // Transform leftLegTwist2Bind = controllerRoot.FindChildRecursive("LeftLegTwist2Bind");
            // Transform leftLegTwist3Bind = controllerRoot.FindChildRecursive("LeftLegTwist3Bind");
            // Transform leftUpLegTwist1Bind = controllerRoot.FindChildRecursive("LeftUpLegTwist1Bind");
            // Transform leftUpLegTwist2Bind = controllerRoot.FindChildRecursive("LeftUpLegTwist2Bind");
            // Transform leftUpLegTwist3Bind = controllerRoot.FindChildRecursive("LeftUpLegTwist3Bind");
            Transform rightUpLeg = controllerRoot.FindChildRecursive("RightUpLeg");
            Transform rightLeg = controllerRoot.FindChildRecursive("RightLeg");
            Transform rightFoot = controllerRoot.FindChildRecursive("RightFoot");
            Transform rightFootBind = controllerRoot.FindChildRecursive("RightFootBind");
            Transform rightToeBase = controllerRoot.FindChildRecursive("RightToeBase");
            // Transform rightToeBaseBind = controllerRoot.FindChildRecursive("RightToeBaseBind");
            // Transform rightLegTwist1Bind = controllerRoot.FindChildRecursive("RightLegTwist1Bind");
            // Transform rightLegTwist2Bind = controllerRoot.FindChildRecursive("RightLegTwist2Bind");
            // Transform rightLegTwist3Bind = controllerRoot.FindChildRecursive("RightLegTwist3Bind");
            // Transform rightUpLegTwist1Bind = controllerRoot.FindChildRecursive("RightUpLegTwist1Bind");
            // Transform rightUpLegTwist2Bind = controllerRoot.FindChildRecursive("RightUpLegTwist2Bind");
            // Transform rightUpLegTwist3Bind = controllerRoot.FindChildRecursive("RightUpLegTwist3Bind");
            Transform spine = controllerRoot.FindChildRecursive("Spine");
            Transform spine1 = controllerRoot.FindChildRecursive("Spine1");
            Transform spine1Bind = controllerRoot.FindChildRecursive("Spine1Bind");
            Transform spine2 = controllerRoot.FindChildRecursive("Spine2");
            Transform leftShoulder = controllerRoot.FindChildRecursive("LeftShoulder");
            Transform leftArm = controllerRoot.FindChildRecursive("LeftArm");
            // Transform leftArmTwist1Bind = controllerRoot.FindChildRecursive("LeftArmTwist1Bind");
            // Transform leftArmTwist2Bind = controllerRoot.FindChildRecursive("LeftArmTwist2Bind");
            // Transform leftArmTwist3Bind = controllerRoot.FindChildRecursive("LeftArmTwist3Bind");
            Transform leftForeArm = controllerRoot.FindChildRecursive("LeftForeArm");
            // Transform leftForeArmTwist1Bind = controllerRoot.FindChildRecursive("LeftForeArmTwist1Bind");
            // Transform leftForeArmTwist2Bind = controllerRoot.FindChildRecursive("LeftForeArmTwist2Bind");
            // Transform leftForeArmTwist3Bind = controllerRoot.FindChildRecursive("LeftForeArmTwist3Bind");
            Transform leftHand = controllerRoot.FindChildRecursive("LeftHand");
            Transform leftHandBind = controllerRoot.FindChildRecursive("LeftHandBind");
            Transform leftHandIndex1 = controllerRoot.FindChildRecursive("LeftHandIndex1");
            //Transform leftHandIndex1Bind = controllerRoot.FindChildRecursive("LeftHandIndex1Bind");
            Transform leftHandIndex2 = controllerRoot.FindChildRecursive("LeftHandIndex2");
            //Transform leftHandIndex2Bind = controllerRoot.FindChildRecursive("LeftHandIndex2Bind");
            Transform leftHandIndex3 = controllerRoot.FindChildRecursive("LeftHandIndex3");
            //Transform leftHandIndex3Bind = controllerRoot.FindChildRecursive("LeftHandIndex3Bind");
            Transform leftHandMiddle1 = controllerRoot.FindChildRecursive("LeftHandMiddle1");
            //Transform leftHandMiddle1Bind = controllerRoot.FindChildRecursive("LeftHandMiddle1Bind");
            Transform leftHandMiddle2 = controllerRoot.FindChildRecursive("LeftHandMiddle2");
            //Transform leftHandMiddle2Bind = controllerRoot.FindChildRecursive("LeftHandMiddle2Bind");
            Transform leftHandMiddle3 = controllerRoot.FindChildRecursive("LeftHandMiddle3");
            //Transform leftHandMiddle3Bind = controllerRoot.FindChildRecursive("LeftHandMiddle3Bind");
            Transform leftHandPinky1 = controllerRoot.FindChildRecursive("LeftHandPinky1");
            //Transform leftHandPinky1Bind = controllerRoot.FindChildRecursive("LeftHandPinky1Bind");
            Transform leftHandPinky2 =  controllerRoot.FindChildRecursive("LeftHandPinky2");
            //Transform leftHandPinky2Bind = controllerRoot.FindChildRecursive("LeftHandPinky2Bind");
            Transform leftHandPinky3 = controllerRoot.FindChildRecursive("LeftHandPinky3");
            //Transform leftHandPinky3Bind = controllerRoot.FindChildRecursive("LeftHandPinky3Bind");
            Transform leftHandRing1 = controllerRoot.FindChildRecursive("LeftHandRing1");
            //Transform leftHandRing1Bind = controllerRoot.FindChildRecursive("LeftHandRing1Bind");
            Transform leftHandRing2 = controllerRoot.FindChildRecursive("LeftHandRing2");
            //Transform leftHandRing2Bind = controllerRoot.FindChildRecursive("LeftHandRing2Bind");
            Transform leftHandRing3 = controllerRoot.FindChildRecursive("LeftHandRing3");
            //Transform leftHandRing3Bind = controllerRoot.FindChildRecursive("LeftHandRing3Bind");
            Transform leftHandThumb1 = controllerRoot.FindChildRecursive("LeftHandThumb1");
            //Transform leftHandThumb1Bind = controllerRoot.FindChildRecursive("LeftHandThumb1Bind");
            Transform leftHandThumb2 = controllerRoot.FindChildRecursive("LeftHandThumb2");
            //Transform leftHandThumb2Bind = controllerRoot.FindChildRecursive("LeftHandThumb2Bind");
            Transform leftHandThumb3 = controllerRoot.FindChildRecursive("LeftHandThumb3");
            // Transform leftHandThumb3Bind = controllerRoot.FindChildRecursive("LeftHandThumb3Bind");
            // Transform leftShoulderBind = controllerRoot.FindChildRecursive("LeftShoulderBind");
            Transform neck = controllerRoot.FindChildRecursive("Neck");
            Transform head = controllerRoot.FindChildRecursive("Head");
            Transform face = controllerRoot.FindChildRecursive("Face");
            //Transform faceBind = controllerRoot.FindChildRecursive("FaceBind");
            Transform jaw = controllerRoot.FindChildRecursive("Jaw");
            // Transform jawBind = controllerRoot.FindChildRecursive("JawBind");
            // Transform leftBrowBind = controllerRoot.FindChildRecursive("LeftBrowBind");
            Transform leftEyeSocket = controllerRoot.FindChildRecursive("LeftEyeSocket");
            Transform leftEyeSocketBind = controllerRoot.FindChildRecursive("LeftEyeSocketBind");
            Transform leftEye = controllerRoot.FindChildRecursive("LeftEye");
            // Transform leftEyeBind = controllerRoot.FindChildRecursive("LeftEyeBind");
            // Transform mouthBind = controllerRoot.FindChildRecursive("MouthBind");
            // Transform mouthInnerBind = controllerRoot.FindChildRecursive("MouthInnerBind");
            // Transform noseBind = controllerRoot.FindChildRecursive("NoseBind");
            // Transform rightBrowBind = controllerRoot.FindChildRecursive("RightBrowBind");
            Transform rightEyeSocket = controllerRoot.FindChildRecursive("RightEyeSocket");
            Transform rightEyeSocketBind = controllerRoot.FindChildRecursive("RightEyeSocketBind");
            Transform rightEye = controllerRoot.FindChildRecursive("RightEye");
            //Transform rightEyeBind = controllerRoot.FindChildRecursive("RightEyeBind");
            Transform neckBind = controllerRoot.FindChildRecursive("NeckBind");
            Transform rightShoulder = controllerRoot.FindChildRecursive("RightShoulder");
            Transform rightArm = controllerRoot.FindChildRecursive("RightArm");
            // Transform rightArmTwist1Bind = controllerRoot.FindChildRecursive("RightArmTwist1Bind");
            // Transform rightArmTwist2Bind = controllerRoot.FindChildRecursive("RightArmTwist2Bind");
            // Transform rightArmTwist3Bind = controllerRoot.FindChildRecursive("RightArmTwist3Bind");
            Transform rightForeArm = controllerRoot.FindChildRecursive("RightForeArm");
            // Transform rightForeArmTwist1Bind = controllerRoot.FindChildRecursive("RightForeArmTwist1Bind");
            // Transform rightForeArmTwist2Bind = controllerRoot.FindChildRecursive("RightForeArmTwist2Bind");
            // Transform rightForeArmTwist3Bind = controllerRoot.FindChildRecursive("RightForeArmTwist3Bind");
            Transform rightHand = controllerRoot.FindChildRecursive("RightHand");
            Transform rightHandBind = controllerRoot.FindChildRecursive("RightHandBind");
            Transform rightHandIndex1 = controllerRoot.FindChildRecursive("RightHandIndex1");
            //Transform rightHandIndex1Bind = controllerRoot.FindChildRecursive("RightHandIndex1Bind");
            Transform rightHandIndex2 = controllerRoot.FindChildRecursive("RightHandIndex2");
            //Transform rightHandIndex2Bind = controllerRoot.FindChildRecursive("RightHandIndex2Bind");
            Transform rightHandIndex3 = controllerRoot.FindChildRecursive("RightHandIndex3");
            //Transform rightHandIndex3Bind = controllerRoot.FindChildRecursive("RightHandIndex3Bind");
            Transform rightHandMiddle1 = controllerRoot.FindChildRecursive("RightHandMiddle1");
            //Transform rightHandMiddle1Bind = controllerRoot.FindChildRecursive("RightHandMiddle1Bind");
            Transform rightHandMiddle2 = controllerRoot.FindChildRecursive("RightHandMiddle2");
            //Transform rightHandMiddle2Bind = controllerRoot.FindChildRecursive("RightHandMiddle2Bind");
            Transform rightHandMiddle3 = controllerRoot.FindChildRecursive("RightHandMiddle3");
            //Transform rightHandMiddle3Bind = controllerRoot.FindChildRecursive("RightHandMiddle3Bind");
            Transform rightHandPinky1 = controllerRoot.FindChildRecursive("RightHandPinky1");
            //Transform rightHandPinky1Bind = controllerRoot.FindChildRecursive("RightHandPinky1Bind");
            Transform rightHandPinky2 =  controllerRoot.FindChildRecursive("RightHandPinky2");
            //Transform rightHandPinky2Bind = controllerRoot.FindChildRecursive("RightHandPinky2Bind");
            Transform rightHandPinky3 = controllerRoot.FindChildRecursive("RightHandPinky3");
            //Transform rightHandPinky3Bind = controllerRoot.FindChildRecursive("RightHandPinky3Bind");
            Transform rightHandRing1 = controllerRoot.FindChildRecursive("RightHandRing1");
            //Transform rightHandRing1Bind = controllerRoot.FindChildRecursive("RightHandRing1Bind");
            Transform rightHandRing2 = controllerRoot.FindChildRecursive("RightHandRing2");
            //Transform rightHandRing2Bind = controllerRoot.FindChildRecursive("RightHandRing2Bind");
            Transform rightHandRing3 = controllerRoot.FindChildRecursive("RightHandRing3");
            //Transform rightHandRing3Bind = controllerRoot.FindChildRecursive("RightHandRing3Bind");
            Transform rightHandThumb1 = controllerRoot.FindChildRecursive("RightHandThumb1");
            //Transform rightHandThumb1Bind = controllerRoot.FindChildRecursive("RightHandThumb1Bind");
            Transform rightHandThumb2 = controllerRoot.FindChildRecursive("RightHandThumb2");
            //Transform rightHandThumb2Bind = controllerRoot.FindChildRecursive("RightHandThumb2Bind");
            Transform rightHandThumb3 = controllerRoot.FindChildRecursive("RightHandThumb3");
            // Transform rightHandThumb3Bind = controllerRoot.FindChildRecursive("RightHandThumb3Bind");
            // Transform rightShoulderBind = controllerRoot.FindChildRecursive("RightShoulderBind");
            // Transform spine2Bind = controllerRoot.FindChildRecursive("Spine2Bind");
            // Transform spineBind = controllerRoot.FindChildRecursive("SpineBind");

            // Find joint pairs manually since we can't rely pre-configured mappings.
            _jointPairs = new JointPair[]
            {
                new JointPair { Joint = root, ParentJoint = null },
                new JointPair { Joint = hips, ParentJoint = root },
                new JointPair { Joint = leftUpLeg, ParentJoint = hips },
                new JointPair { Joint = leftLeg, ParentJoint = leftUpLeg },
                new JointPair { Joint = leftFoot, ParentJoint = leftLeg },
                new JointPair { Joint = leftFootBind, ParentJoint = leftFoot },
                new JointPair { Joint = leftToeBase, ParentJoint = leftFootBind },
                new JointPair { Joint = rightUpLeg, ParentJoint = hips },
                new JointPair { Joint = rightLeg, ParentJoint = rightUpLeg },
                new JointPair { Joint = rightFoot, ParentJoint = rightLeg },
                new JointPair { Joint = rightFootBind, ParentJoint = rightFoot },
                new JointPair { Joint = rightToeBase, ParentJoint = rightFootBind },
                new JointPair { Joint = spine, ParentJoint = hips },
                new JointPair { Joint = spine1, ParentJoint = spine },
                new JointPair { Joint = spine1Bind, ParentJoint = spine1 },
                new JointPair { Joint = spine2, ParentJoint = spine1 },
                new JointPair { Joint = leftShoulder, ParentJoint = spine2 },
                new JointPair { Joint = leftArm, ParentJoint = leftShoulder },
                new JointPair { Joint = leftForeArm, ParentJoint = leftArm },
                new JointPair { Joint = leftHand, ParentJoint = leftForeArm },
                new JointPair { Joint = leftHandBind, ParentJoint = leftHand },
                new JointPair { Joint = leftHandIndex1, ParentJoint = leftHandBind },
                new JointPair { Joint = leftHandIndex2, ParentJoint = leftHandIndex1 },
                new JointPair { Joint = leftHandIndex3, ParentJoint = leftHandIndex2 },
                new JointPair { Joint = leftHandMiddle1, ParentJoint = leftHandBind },
                new JointPair { Joint = leftHandMiddle2, ParentJoint = leftHandMiddle1 },
                new JointPair { Joint = leftHandMiddle3, ParentJoint = leftHandMiddle2 },
                new JointPair { Joint = leftHandPinky1, ParentJoint = leftHandBind },
                new JointPair { Joint = leftHandPinky2, ParentJoint = leftHandPinky1 },
                new JointPair { Joint = leftHandPinky3, ParentJoint = leftHandPinky2 },
                new JointPair { Joint = leftHandRing1, ParentJoint = leftHandBind },
                new JointPair { Joint = leftHandRing2, ParentJoint = leftHandRing1 },
                new JointPair { Joint = leftHandRing3, ParentJoint = leftHandRing2 },
                new JointPair { Joint = leftHandThumb1, ParentJoint = leftHandBind },
                new JointPair { Joint = leftHandThumb2, ParentJoint = leftHandThumb1 },
                new JointPair { Joint = leftHandThumb3, ParentJoint = leftHandThumb2 },
                new JointPair { Joint = neck, ParentJoint = spine2 },
                new JointPair { Joint = head, ParentJoint = neck },
                new JointPair { Joint = face, ParentJoint = head },
                new JointPair { Joint = jaw, ParentJoint = face },
                new JointPair { Joint = leftEyeSocket, ParentJoint = face },
                new JointPair { Joint = leftEyeSocketBind, ParentJoint = leftEyeSocket },
                new JointPair { Joint = leftEye, ParentJoint = leftEyeSocketBind },
                new JointPair { Joint = rightEyeSocket, ParentJoint = face },
                new JointPair { Joint = rightEyeSocketBind, ParentJoint = rightEyeSocket },
                new JointPair { Joint = rightEye, ParentJoint = rightEyeSocketBind },
                new JointPair { Joint = neckBind, ParentJoint = neck },
                new JointPair { Joint = rightShoulder, ParentJoint = spine2 },
                new JointPair { Joint = rightArm, ParentJoint = rightShoulder },
                new JointPair { Joint = rightForeArm, ParentJoint = rightArm },
                new JointPair { Joint = rightHand, ParentJoint = rightForeArm },
                new JointPair { Joint = rightHandBind, ParentJoint = rightHand },
                new JointPair { Joint = rightHandIndex1, ParentJoint = rightHandBind },
                new JointPair { Joint = rightHandIndex2, ParentJoint = rightHandIndex1 },
                new JointPair { Joint = rightHandIndex3, ParentJoint = rightHandIndex2 },
                new JointPair { Joint = rightHandMiddle1, ParentJoint = rightHandBind },
                new JointPair { Joint = rightHandMiddle2, ParentJoint = rightHandMiddle1 },
                new JointPair { Joint = rightHandMiddle3, ParentJoint = rightHandMiddle2 },
                new JointPair { Joint = rightHandPinky1, ParentJoint = rightHandBind },
                new JointPair { Joint = rightHandPinky2, ParentJoint = rightHandPinky1 },
                new JointPair { Joint = rightHandPinky3, ParentJoint = rightHandPinky2 },
                new JointPair { Joint = rightHandRing1, ParentJoint = rightHandBind },
                new JointPair { Joint = rightHandRing2, ParentJoint = rightHandRing1 },
                new JointPair { Joint = rightHandRing3, ParentJoint = rightHandRing2 },
                new JointPair { Joint = rightHandThumb1, ParentJoint = rightHandBind },
                new JointPair { Joint = rightHandThumb2, ParentJoint = rightHandThumb1 },
                new JointPair { Joint = rightHandThumb3, ParentJoint = rightHandThumb2 },
            };

            // Assign the config asset here to avoid errors/warnings on Awake()
            ConfigAsset = Resources.Load<TextAsset>("GenericGenie/Genie");

            // Initialize empty processor containers to avoid null refs.
            _sourceProcessorContainers = new SourceProcessorContainer[0];
            _targetProcessorContainers = new TargetProcessorContainer[0];

            base.Awake();
        }

        public void SetUpForLocomotion(Transform ovrRigRoot, ManagedAvatar avatar, FirstPersonLocomotor locomotor, RuntimeAnimatorController locomotionAnimatorController)
        {
            avatar.Animator.runtimeAnimatorController = locomotionAnimatorController;

             var newProcessors = new GeniesTargetProcessorContainer[2]
            {
                new GeniesTargetProcessorContainer(),
                new GeniesTargetProcessorContainer(),
            };

            newProcessors[0].SetAsAnimationTargetProcessor();
            newProcessors[1].SetAsLocomotionTargetProcessor(ovrRigRoot, avatar, locomotor );

            _targetProcessorContainers = newProcessors;

            Debug.Log("Locomotion Event Handler2: " + newProcessors[1].LocomotionProcessor.LocomotionEventHandler);

            newProcessors[1].LocomotionProcessor.Initialize(this);
        }
    }

    public class GeniesTargetProcessorContainer : TargetProcessorContainer
    {
        public LocomotionSkeletalProcessor LocomotionProcessor => _locomotionProcessor;

        public void SetAsAnimationTargetProcessor()
        {
            _currentProcessorType = TargetProcessor.ProcessorType.Animation;

            _animationProcessor = new AnimationSkeletalProcessor()
            {
                Weight = 0.0f,
            };

            // Apply lower body blend indices.
            _animationProcessor.AnimBlendIndices = new TargetJointIndex[]
            {
                new TargetJointIndex(2),
                new TargetJointIndex(3),
                new TargetJointIndex(4),
                new TargetJointIndex(5),
                new TargetJointIndex(6),
                new TargetJointIndex(7),
                new TargetJointIndex(8),
                new TargetJointIndex(9),
                new TargetJointIndex(10),
                new TargetJointIndex(11),
            };
        }

        public void SetAsLocomotionTargetProcessor(Transform _ovrCameraRigRoot, ManagedAvatar avatar, FirstPersonLocomotor locomotor)
        {
            _currentProcessorType = TargetProcessor.ProcessorType.Locomotion;
            _locomotionProcessor = new LocomotionSkeletalProcessor()
            {
                Weight = 1.0f,
                //LocomotionEventHandler = locomotor,
                CameraRig = _ovrCameraRigRoot,
                Animator = avatar.Animator,
                // AnimatorHorizontalParam = "Horizontal",
                AnimatorVerticalParam = "Vertical",
                AnimationSpeed = 2,
            };

            // There's a mistake in the Meta SDK that causes AnimatorHorizontalParam to also be a getter/setter for AnimatorVerticalParam. To get around this,
            // we have to use reflection to set the horizontal param.
            var horizontalField = typeof(LocomotionSkeletalProcessor).GetField("_animatorHorizontalParam", BindingFlags.Instance | BindingFlags.NonPublic);
            if (horizontalField == null)
            {
                Debug.LogError("Meta LocomotionSkeletalProcessor no longer has a private field named '_animatorHorizontalParam'. Update reflection lookup.");
                return;
            }

            horizontalField.SetValue(_locomotionProcessor, "Horizontal");

            // In addition, we have to set the private field for the ILocomotionEventHandler since setting the public property doesn't work correctly.
            var locomotionHandlerObjectField = typeof(LocomotionSkeletalProcessor).GetField("_locomotionEventHandlerObject", BindingFlags.Instance | BindingFlags.NonPublic);
            if (locomotionHandlerObjectField == null)
            {
                Debug.LogError("Meta LocomotionSkeletalProcessor no longer has a private field named '_locomotionEventHandlerObject'. Update reflection lookup.");
                return;
            }

            locomotionHandlerObjectField.SetValue(_locomotionProcessor, locomotor);
        }
    }
}

