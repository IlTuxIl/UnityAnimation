﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.ComponentModel;
using UnityEngine.Networking.Types;
using System;

public class PlayAnimationEditor : EditorWindow
{
	// Current GameObject Selected on the Scene
	[SerializeField]
	protected GameObject m_skeleton;

	// List of all body joints of the current Skeleton
	// Transform -> Unity Type (Position, Rotation, Scale)
	// Matrix Transform
	[SerializeField]
	public List<Transform> m_BodyJoints;

	// Current AnimationClip Selected by the user
	// This AnimationClip will be played 
	[SerializeField]
	protected AnimationClip m_animationClip;


	// Boolean to check if the animation is playing or not
	private bool m_b_isRunning = false;

	// Stocks the current Time
	protected float m_f_time = 0.0f;

	// Vector2 used to get the save the position of the scroll in the Window
	Vector2 m_scrollPostion = Vector2.zero;

	// First frame in seconds (here it will be 0.0f)
	private float m_f_startTime = 0.0f;
	// Length of the AnimationClip in seconds 
	private float m_f_endTime;
	// Frame Duration in seconds
	private float m_f_frameDuration;
	// accelerate or slow the animation
	protected float m_f_scaleTime = 1f;


	// Dictionnary of string & List<Vector3> which contains all the position of one body Joint
	private Dictionary<string, List<Vector3>> m_trajectories;

	// Dictionnary of string & bool which contains all the bool used by the toggle button
	protected Dictionary<string, bool> m_toggleTraj;

	// The new item in the menu
	[MenuItem("Window/PlayAnimation", false, 2000)]
	public static void DoWindow()
	{
		PlayAnimationEditor m_window = GetWindow<PlayAnimationEditor>();       
	}

	// Called when the user change the selected object in the scene windows
	public void OnSelectionChange()
	{
		// Pick the current selected gameObject in the Scene.
		m_skeleton = Selection.activeGameObject;
		// The user can pick in the void
		if (m_skeleton != null)
		{
			// Check if the current gameObject has an animator component or an animation            
			if (m_skeleton.GetComponent<Animation>() || m_skeleton.GetComponent<Animator>())
			{
				if (m_BodyJoints == null)
					m_BodyJoints = new List<Transform>();

				// Get all the bodyJoints -> This is specific to the skeleton that i used
				// One body joint is defined when 
				m_BodyJoints = m_skeleton.GetComponentsInChildren<Transform>().Where(x => x.childCount != 0).ToList();
				//Repaint the currentWindow -> Call the OnGui function
				Repaint();
			}
		}      
	}



	// This function enables the editor to handle an event in the scene view.
	// We need to redraw the curve & points when the user is interacting with the SceneView
	// In this function, you will have to code the drawing of the trajectories
	public void OnSceneGUI(SceneView sceneView)
	{
		// TODO
		// Parcourir toutes les articulations
		//    Si la togglebox est cochée
		//       Parcourir tous les points de la trajectoire et les afficher
		// Vous pourrez appeler la fonction de dessin d'un point et/ou Handles.DrawLine(l_oldPoint, l_currentPoint);

		if (m_trajectories == null)
			return;
		if (m_toggleTraj == null)
			return;
		if (m_skeleton == null)
			return;

		for(int i = 0; i < m_BodyJoints.Count; ++i){
			if (m_toggleTraj [m_BodyJoints [i].name]) {
				List<Vector3> tmp = m_trajectories [m_BodyJoints [i].name];
				for (int j = 0; j < tmp.Count - 1; ++j)
					Handles.DrawLine (tmp [j], tmp [j + 1]);
			}
		}
	}


	// Init the trajectories for each body joints for the current Animation Clip
	private void initTrajectories()
	{
		// TODO
		// Créer ou vider dictionnaire m_trajectories qui va contenir la list des points de la trajectoire
		// Créer ou vider dictionnaire m_toggleTraj qui va contenir la list des bool indiquant si la trajectoire est visible ou non
		// Remplir le m_trajectories[m_BodyJoints[i].name] avec les positions des points
		// Voir AnimationMode.BeginSampling(); .... AnimationMode.EndSampling(); qui se trouve dans playAnimation
		if(m_trajectories == null)
			m_trajectories = new Dictionary<string, List<Vector3>>();

		if (m_toggleTraj == null)
			m_toggleTraj = new Dictionary<string, bool> ();

		m_trajectories.Clear();
		m_toggleTraj.Clear();

		m_f_endTime = m_animationClip.length;
		m_f_frameDuration = 1.0f / m_animationClip.frameRate;

		if (!AnimationMode.InAnimationMode ())
			AnimationMode.StartAnimationMode ();

		for (int i = 0; i < m_BodyJoints.Count; ++i) {
			List<Vector3> tmp = new List<Vector3>();

			for (double j = m_f_startTime; j < m_f_endTime; j += m_f_frameDuration) {
				samplePosture ((float)j);
				tmp.Add(m_BodyJoints[i].transform.position);
			}

			m_trajectories.Add(m_BodyJoints[i].name, tmp);
			m_toggleTraj.Add(m_BodyJoints[i].name, true);
		}
	}


	// OnGUI is called for rendering and handling GUI events.
	// Use OnGUI to draw all the controls of your window.
	public void OnGUI()
	{
		//if (m_toggleTraj != null) m_toggleTraj.Clear();

		// We need to select a GameObject in the Scene
		if (m_skeleton == null)
		{
			EditorGUILayout.HelpBox("Please select a GameObject.", MessageType.Info);
			return;
		}
		// Check if the current GameObject is active
		if (!m_skeleton.active)
		{
			EditorGUILayout.HelpBox("Please select a GameObject that is active.", MessageType.Info);
			return;
		}
		// Check if the current GameObject has an Animator or Animation Component
		if (m_skeleton.GetComponent<Animator>() == null && m_skeleton.GetComponent<Animation>() == null)
		{
			EditorGUILayout.HelpBox("Please select a GameObject with an Animator Component or Animation.", MessageType.Info);
			return;
		}

		// Update the scroll Position
		m_scrollPostion = GUILayout.BeginScrollView(m_scrollPostion, false, false);

		// Begin a vertical group that will contains all the gui element we will declare between the BeginVertical and the EndVertical
		EditorGUILayout.BeginVertical();

		// Create a "Listener" on the next GUI Element in order to track some change on it
		EditorGUI.BeginChangeCheck();
		// Create an Object Field of type AnimationClip
		// This allows the user to select which AnimationClip he wants to play
		m_animationClip = EditorGUILayout.ObjectField("Current Animation Clip", m_animationClip, typeof(AnimationClip), false) as AnimationClip;
		// If the animation clip has changed, we need to compute the new Trajectories !
		if (EditorGUI.EndChangeCheck())
		{
			initTrajectories();
		}

		// If the user has selected an AnimationClip
		if (m_animationClip != null)
		{

			if (GUILayout.Button ("Gaussian filter"))
				GaussianAnim ();

			if (GUILayout.Button ("Edit")) {
				GaussianAnim ();
				//GUILayout.Slider(
			}



			// Get the Length of the current Animation
			m_f_endTime = m_animationClip.length;
			// Get the frame Duration of the current Animation
			m_f_frameDuration = 1.0f / m_animationClip.frameRate;
			// An example for a Slider with change detect
			// So we need to call The  EditorGUI.BeginChangeCheck() in order to create a "Listener"
			EditorGUI.BeginChangeCheck();
			// Then we create the Object that we want to track some change on 
			m_f_time = EditorGUILayout.Slider("Time (seconds)", m_f_time, m_f_startTime, m_f_endTime);
			// If the user has modified the Slider Precision here, we can detect it and call a fonction for example
			if (EditorGUI.EndChangeCheck())
				samplePosture(m_f_time);

			EditorGUI.BeginChangeCheck();
			// Then we create the Object that we want to track some change on 
			m_f_scaleTime = EditorGUILayout.Slider("Scale Time", m_f_scaleTime, 0.0f, 2.0f);

			if (!m_b_isRunning)
			{
				// Create a Button in order to plays the AnimationClip
				if (GUILayout.Button("Start Animation"))
				{
					// Starts the Coroutine that will play the Animation
					//Swing.Editor.EditorCoroutine.start(repeatAnimation(m_f_frameDuration));
					// Coroutine is runnning
					m_b_isRunning = true;
				}
			}
			else
			{
				// Stop the Coroutine
				if (GUILayout.Button("Stop Animation"))
				{
					// Swing.Editor.EditorCoroutine.stop(repeatAnimation(m_f_frameDuration));
					m_b_isRunning = false;
				}
			}


			// TODO IHM : 
			// faire un for sur les articulations, creer des boites a cocher pour chaque articulation
			// Voir la doc : https://docs.unity3d.com/ScriptReference/EditorGUILayout.Toggle.html
			// Utilisez la variables : m_toggleTraj[m_BodyJoints[i].name]

			for (int i = 0; i < m_BodyJoints.Count; ++i) {
				bool tmp = m_toggleTraj[m_BodyJoints[i].name];
				tmp = EditorGUILayout.Toggle(m_BodyJoints[i].name, tmp);
				m_toggleTraj[m_BodyJoints[i].name] = tmp;

				/*EditorCurveBinding binding in AnimationUtility.GetCurveBindings(m_animationClip)
				AnimationCurve curve = AnimationUtility.GetEditorCurve(m_animationClip, binding);
				EditorGUILayout.CurveField("Animation on X", curve);
				*/
            }
			Debug.Log (m_BodyJoints.Count);
		}

		// End the vertical group
		EditorGUILayout.EndVertical();

		// Stop the Scroll
		GUILayout.EndScrollView();
	}


	// Call at each frame
	// In this function, we will play the Animation
	private void Update()
	{
		// TODO
		// Verifier que m_skeleton m_animationClip, m_b_isRunning sont init
		// modifier le temps : m_f_time
		// appeler samplePosture qui est ue fonction un peu plus bas

		if (m_skeleton == null)
			return;
		if (m_animationClip == null)
			return;
		if (m_b_isRunning == false)
			return;

		
		samplePosture (m_f_time);
		m_f_time += m_f_frameDuration * m_f_scaleTime;

		if (m_f_time >= m_f_endTime)
			m_f_time = m_f_startTime;

		//if (!AnimationMode.InAnimationMode ())
		//	AnimationMode.StartAnimationMode ();


		SceneView.RepaintAll();
	}


	// Sample our Skeleton at the time given in parameter for the currentAnimationClip
	private void samplePosture(float p_f_time)
	{
		// Check if the Game isn't running & the Animation Mode is enabled
		if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
		{
			// We need to BeginSampling before the SampleAnimationClip is called
			AnimationMode.BeginSampling();
			// Samples the animationClip (m_animation) at the time (m_f_time) for the skeleton (m_skeleton)
			// If the GameObject & the AnimationClip are different -> no errors are trigger but nothing happen 
			AnimationMode.SampleAnimationClip(m_skeleton, m_animationClip, p_f_time);
			// Ending the Sampling of the Animation
			AnimationMode.EndSampling();
			// Repaint The SceneView as the skeleton has changed
			SceneView.RepaintAll();
			// Repaint the GUI as we are changing the variable m_f_time on which we have a slider
			Repaint();
		}
	}

	void OnFocus()
	{
		// Remove delegate listener if it has previously
		// been assigned.
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		// Add (or re-add) the delegate.
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDestroy()
	{
		// When the window is destroyed, remove the delegate
		// so that it will no longer do any drawing.
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

    public void GaussianAnim()
    {
        AnimationClip clip = new AnimationClip();  // comportera la copie de m_animationClip mais filtrer

        // Copy the m_animationClip in the local variale clip
        clip.legacy = m_animationClip.legacy;

        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(m_animationClip)){

            AnimationCurve curve = AnimationUtility.GetEditorCurve(m_animationClip, binding);

            //TODO : editer chaque courbe ici avec
            // Parcourir toute la courbe avec comme longueur : curve.length
            //  float v = curve.keys[time].value;
            //  curve.MoveKey(time, new Keyframe(time, v));
            int fenetre = 5;
            double[] gaussianClip = new double[curve.length];

            for (int i = fenetre; i < curve.length - fenetre; i++){
                double val = 0, som = 0;

                for (int j = -fenetre; j <= fenetre; j++){
                    double coeff = (1.0 / Math.Sqrt(2.0 * Math.PI)) * Math.Exp(-((j * j) / 2.0));

                    val += coeff * curve.keys[i + j].value;
                    som += coeff;
                }

                gaussianClip[i] = (val / som);
            }

            for (int i = fenetre; i < curve.length - fenetre; i++)
            	curve.MoveKey(i, new Keyframe(curve.keys[i].time, (float)gaussianClip[i]));
            
			AnimationUtility.SetEditorCurve(clip, binding, curve);
        }

        // Si vous avez besoin de (quaternion+translation) il faut les regrouper les courbes
        // il y a 7 courbes par articulations, par exemple pour le noeud "root" il y a
        // "rootT.x"  "rootT.y" "rootT.z" pour la translation
        // "rootQ.x"  "rootQ.y" "rootQ.z" "rootQ.w" pour le quaternion
        // Il faut donc regrouper ces 4 courbes en un tableau de quaternion

        // Save the local variale clip
        AssetDatabase.CreateAsset(clip, "Assets/Gaussian" + m_animationClip.name + ".anim");
    }

	void multiResDecompose(){

		List<float> tab = new List<float>();
		List<List<float>> listMoy = new List<List<float>>();
		List<List<float>> listEcart = new List<List<float>>();

		listMoy.Add(new List<float>());
		listEcart.Add(new List<float>());

		for (int i = 0; i < tab.Count; ++i) {
			listMoy [i].Add (tab [i]);
			listEcart [i].Add (0.0f);
		}

		int cpt = 1;
		while (listMoy [cpt - 1].Count > 1) {

			listMoy.Add(new List<float>());
			listEcart.Add(new List<float>());

			for (int i = 0; i < listMoy[cpt-1].Count - 2; i += 2) {
				float moy = (tab [i] + tab [i + 1]) / 2;
				float ecart = tab [i] - moy;
				float ecart2 = tab [i+1] - moy;

				listMoy [cpt].Add (moy);
				listEcart [cpt].Add (ecart);
				listEcart [cpt].Add (ecart2);

			}
			cpt++;
		}
	}

	void multiResRecompose(List<float> coeffs){
		List<List<float>> listMoy = new List<List<float>>();;
		List<List<float>> listEcart = new List<List<float>>();

		listMoy.Add(new List<float>());
		listEcart.Add(new List<float>());
		for (int j = coeffs.Count - 1; j >= 0; --j) {
			int k = 0;
			for (int i = 0; i < listMoy [j-1].Count - 2; i += 2) {

				listMoy [j-1][i] = listMoy [j][i] - (listEcart[j][k] * coeffs[j]);
				++k;
				listMoy [j-1][i+1] = listMoy [j][i] + (listEcart[j][k] * coeffs[j]);
				++k;
			}

		}
	}

}
