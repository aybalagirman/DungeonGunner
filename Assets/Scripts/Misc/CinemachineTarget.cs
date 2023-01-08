using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

	private void Awake()
	{
		// load component
		cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
	}

	// start is called before the first frame update
	private void Start()
	{
		SetCinemachineTargetGroup();
	}

	// set the cinemachine camera target group
	private void SetCinemachineTargetGroup()
	{
		// create the target group for cinemachine for the cinemachine camera to follow
		CinemachineTargetGroup.Target cinemachineGroupTargetPlayer = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = GameManager.Instance.GetPlayer().transform };
		CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[] { cinemachineGroupTargetPlayer };

		cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
	}
}
