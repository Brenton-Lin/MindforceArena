using UnityEngine;

namespace VRArmIK
{
	[ExecuteInEditMode]
	public class PoseManager : MonoBehaviour
	{
		
		public VRTrackingReferences vrTransforms;

		public delegate void OnCalibrateListener();

		public event OnCalibrateListener onCalibrate;
		public float headsetSpecificWristWidthRatio = 0.74f;
		public const float referencePlayerHeightHmd = 1.7f;
		public const float referencePlayerWidthWrist = 1.39f;
		public const float referencePlayerWidthShoulders = 0.2f;
		public float playerHeightHmd = 1.70f;
		public float playerWidthWrist = 1.39f;
		public float playerWidthShoulders = 0.2f;


		void OnEnable()
		{
			
		}

		void Awake()
		{
			//loadPlayerSize();
		}

		void Start()
		{
			onCalibrate += OnCalibrate;
		}

		[ContextMenu("Set Player Height From Scene")]
		void OnCalibrate()
		{
			playerHeightHmd = Camera.main.transform.position.y;
		}

		void loadPlayerWidthShoulders()
		{
			playerWidthShoulders = PlayerPrefs.GetFloat("VRArmIK_PlayerWidthShoulders", 0.2f);
		}

		

		[ContextMenu("SavePlayerSize")]
		public void calibrateIK()
		{
			//bypass automatic calibration?
			//playerWidthWrist = (vrTransforms.leftHand.position - vrTransforms.rightHand.position).magnitude*(headsetSpecificWristWidthRatio);
			//playerHeightHmd = vrTransforms.hmd.position.y;
			savePlayerSize(playerHeightHmd, playerWidthWrist, playerWidthShoulders);
		}

		
		public void savePlayerSize(float heightHmd, float widthWrist, float widthShoulders)
		{
			PlayerPrefs.SetFloat("VRArmIK_PlayerHeightHmd", heightHmd);
			PlayerPrefs.SetFloat("VRArmIK_PlayerWidthWrist", widthWrist);
			PlayerPrefs.SetFloat("VRArmIK_PlayerWidthShoulders", widthShoulders);
			loadPlayerSize();
			onCalibrate?.Invoke();
		}

		public void networkPlayerSizeChanged(){
			onCalibrate?.Invoke();
		}

		public void loadPlayerSize()
		{
			playerHeightHmd = PlayerPrefs.GetFloat("VRArmIK_PlayerHeightHmd", referencePlayerHeightHmd);
			playerWidthWrist = PlayerPrefs.GetFloat("VRArmIK_PlayerWidthWrist", referencePlayerWidthWrist);
			playerWidthShoulders = PlayerPrefs.GetFloat("VRArmIK_PlayerWidthShoulders", referencePlayerWidthShoulders);
		}
	}
}