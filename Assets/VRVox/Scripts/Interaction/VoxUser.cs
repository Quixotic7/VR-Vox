using System;
using UnityEngine;
using System.Collections;

namespace VRVox
{
	[Serializable]
	public class VoxPrefab
	{
		public Vector3[] BlockPositions;

		protected WorldPos[] WorldPositions;

		protected bool WorldPositionsCalculated = false;

		public void RecalculateWorldPositions()
		{
			WorldPositions = new WorldPos[BlockPositions.Length];

			for (var i = 0; i < BlockPositions.Length; i++)
			{
				var b = BlockPositions[i];
				WorldPositions[i] = new WorldPos((int)b.x, (int)b.y, (int)b.z);
			}

			WorldPositionsCalculated = true;
		}

		public WorldPos[] GetWorldPositions()
		{
			if(!WorldPositionsCalculated) RecalculateWorldPositions();
			return WorldPositions;
		}
	}

	public class VoxUser : MonoBehaviour
	{
		public enum VoxInteractionMode 
		{
			Create,
			Paint,
			Teleport
		}

		public VoxInteractionMode interactionMode = VoxInteractionMode.Create;

		public LayerMask Layers;

		public VoxCrosshair Crosshair;

		public World VoxWorld;

		public bool replaceBlocksWhileDrawing = true;

	    public bool mouseLeftRightSwitchesAddAndSubtract = false; // Use this for PC if using a two button mouse

		public float RayCastDistance = 50f;

		public float ActivateDrawTime = 0.2f;

		protected float DrawTimer;

		protected float createTimer;
		protected float destroyTimer;
		protected bool lastSelect;
		protected Vector2 touchTracker;

		//protected bool AddMode = true;

		protected WorldPos startBlockPos;

		protected WorldPos lastBlockPos;

		protected Vector3 startRaycastPos;

		protected Plane constructionPlane;

		protected Plane adjacentPlane;

		public Color NewBlockColor;

		public float ForwardDot;
		public float UpDot;
		public float RightDot;

		public VoxMenu MainMenu;

		public VoxMenuButton AddButton;
		public VoxMenuButton SubtractButton;
		public VoxMenuButton PaintButton;
		//public VoxMenuButton PrefabButton;
		public VoxMenuButton TeleportButton;

		public Transform PlayerObject;

		protected bool lastClickDown;

		public int PrefabIndex;

		public VoxPrefab[] Prefabs;

		public bool drawStraight = false;
		public bool doubleClickToIncreaseHeight = false;
		public bool paintUsingConstructionPlanes = true;
		public bool createAtFixedDistance = false;


		public VoxMenuButton DrawStraightButton;
		public VoxMenuButton DoubleClickLayerButton;
		public VoxMenuButton PaintUsingPlanesButton;
		public VoxMenuButton CreateAtFixedDistanceButton;

		public VoxMenuButton[] BrushButtons;

		public AudioSource audioSource;

		public AudioClip sndCreate;
		public AudioClip sndCreateDrag;

		public AudioClip sndDelete;
		public AudioClip sndDeleteDrag;

		public AudioClip sndTeleport;
		public AudioClip sndPaint;
		public AudioClip sndPaintDrag;


		protected bool AddMode = true;
		protected bool UsePrefab = false;

		public float clipRetriggerTime;
		protected float audioClipTimer;

		public void PlayClip(AudioClip clip, bool canRetrigger = false)
		{
			if (!canRetrigger && audioSource.isPlaying)
			{
				if (audioClipTimer < clipRetriggerTime)
				{
					return;
				}
			}

			audioSource.clip = clip;
			audioSource.Play();
			audioClipTimer = 0;
			//audioSource.PlayOneShot(clip);
		}

		protected void OnAddButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			//Debug.Log("Adding");

			//interactionMode = VoxInteractionMode.AddBlocks;

			interactionMode = VoxInteractionMode.Create;

			AddMode = true;

			AddButton.isSelected = true;
			SubtractButton.isSelected = false;
			PaintButton.isSelected = false;
		}

		protected void OnSubtractButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			//Debug.Log("Subtracting");
			//interactionMode = VoxInteractionMode.SubtractBlocks;

			interactionMode = VoxInteractionMode.Create;

			AddMode = false;

			SubtractButton.isSelected = true;
			AddButton.isSelected = false;
			PaintButton.isSelected = false;
		}

		protected void OnPaintButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			//Debug.Log("Painting");
			//interactionMode = VoxInteractionMode.PaintBlocks;
			//AddMode = false;

			if (button.isSelected)
			{
				interactionMode = VoxInteractionMode.Paint;

				PaintButton.isSelected = true;
				AddButton.isSelected = false;
				SubtractButton.isSelected = false;
			}
			else
			{
				interactionMode = VoxInteractionMode.Create;

				if (AddMode)
				{
					AddButton.isSelected = true;
					SubtractButton.isSelected = false;
				}
				else
				{
					AddButton.isSelected = false;
					SubtractButton.isSelected = true;
				}
			}
		}

		protected void OnPrefabButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			//Debug.Log("Prefab");
			//interactionMode = VoxInteractionMode.Prefab;

			//interactionMode = VoxInteractionMode.Create;

			UsePrefab = button.isSelected;

			if (!UsePrefab) PrefabIndex = 0;
			else PrefabIndex = 1;

			//AddMode = false;
		}


		protected void OnBrushButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			//Debug.Log("Prefab");
			//interactionMode = VoxInteractionMode.Prefab;

			//interactionMode = VoxInteractionMode.Create;

			//UsePrefab = button.isSelected;

			//if (!UsePrefab) PrefabIndex = 0;
			//else PrefabIndex = 1;

			var newIndex = 0;

			if (int.TryParse(button.description, out newIndex))
			{
				PrefabIndex = newIndex;
			}

			//AddMode = false;
		}

		protected void OnTeleportButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			//Debug.Log("Adding");

			//interactionMode = VoxInteractionMode.AddBlocks;

			interactionMode = VoxInteractionMode.Teleport;

			AddButton.isSelected = false;
			SubtractButton.isSelected = false;
			PaintButton.isSelected = false;
			//PrefabButton.isSelected = false;
		}

		protected void OnDrawStraightButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			drawStraight = button.isSelected;
		}

		protected void OnDoubleClickLayerButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			doubleClickToIncreaseHeight = button.isSelected;
		}

		protected void OnPaintUsingPlanesButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			paintUsingConstructionPlanes = button.isSelected;
		}

		protected void OnCreateAtFixedDistanceButton(VoxMenuButton button, bool state)
		{
			if (state == false) return;

			createAtFixedDistance = button.isSelected;
		}

		void Awake()
		{
			//if (cameraController == null)
			//{
			//	Debug.LogError("ERROR: missing camera controller object on " + name);
			//	enabled = false;
			//	return;
			//}
			// clone the crosshair material
			//crosshairMaterial = GetComponent<Renderer>().material;


			AddButton.SubscribeToButton(OnAddButton);
			SubtractButton.SubscribeToButton(OnSubtractButton);
			PaintButton.SubscribeToButton(OnPaintButton);
			//PrefabButton.SubscribeToButton(OnPrefabButton);
			TeleportButton.SubscribeToButton(OnTeleportButton);

			DrawStraightButton.SubscribeToButton(OnDrawStraightButton);
			DoubleClickLayerButton.SubscribeToButton(OnDoubleClickLayerButton);
			PaintUsingPlanesButton.SubscribeToButton(OnPaintUsingPlanesButton);
			CreateAtFixedDistanceButton.SubscribeToButton(OnCreateAtFixedDistanceButton);

			DrawStraightButton.isSelected = drawStraight;
			DoubleClickLayerButton.isSelected = doubleClickToIncreaseHeight;
			PaintUsingPlanesButton.isSelected = paintUsingConstructionPlanes;
			CreateAtFixedDistanceButton.isSelected = createAtFixedDistance;

			foreach (var button in BrushButtons)
			{
				button.SubscribeToButton(OnBrushButton);
			}

			constructionPlane = new Plane(Vector3.up, new Vector3(0,0,0));
			adjacentPlane = new Plane(Vector3.up, new Vector3(0,0, 0));


			AddButton.isSelected = true;
			SubtractButton.isSelected = false;
			PaintButton.isSelected = false;

			//foreach (var prefab in Prefabs)
			//{
			//	prefab.RecalculateWorldPositions();
			//}

		}

		protected static void GetMaxValue(float[] values, out int locationIndex, out float maxValue)
		{
			maxValue = values[0];
			locationIndex = 0;
			for (var i = 1; i < values.Length; i++)
			{
				if (!(values[i] > maxValue)) continue;
				maxValue = values[i];
				locationIndex = i;
			}
		}

		public static Plane GetXYZPlaneFacingDirection(Vector3 direction, Vector3 point)
		{
			var dotProducts = new[]
			{
				Mathf.Abs(Vector3.Dot(direction, Vector3.forward)),
				Mathf.Abs(Vector3.Dot(direction, Vector3.right)),
				Mathf.Abs(Vector3.Dot(direction, Vector3.up))
			};

			int loc;
			float max;
			GetMaxValue(dotProducts, out loc, out max);

			switch (loc)
			{
				case 0:
					return new Plane(Vector3.forward, point);
				case 1:
					return new Plane(Vector3.right, point);
				case 2:
					return new Plane(Vector3.up, point);
			}
			return new Plane(Vector3.up, point);
		}


		public float doubleClickDelay = 0.25f;

		protected float clickHeldTimer = 0f;
		protected float doubleClickTimer = 0f;

		protected bool checkModify;

		protected bool drawStarted = false;
		protected bool inverseAdd;
		protected bool drawSecondLayer;

		protected Vector3 constructionBlockPos;

		protected WorldPos startWorldPos;
		protected WorldPos secondBlockPos;
		protected bool secondBlockDrawn;

		//protected WorldPos lastBlockPos;

		protected float minBuildDistance = 0.5f;

		protected float lastDPadX;

		protected float fixedDistance;

		protected void InterfaceWithBlocks()
		{
			RaycastHit hit;

			var viewPos = VoxManager.ViewPosition;
			var viewForward = VoxManager.ViewForward;

			var ray = VoxManager.ViewRay;

			var blocksModified = false;

			var click = false;
			var clickDown = false;
			var clickUp = false;

			if (Input.GetKeyDown(KeyCode.F2))
			{
//				OVRManager.display.RecenterPose();
                UnityEngine.VR.InputTracking.Recenter();
			}

			if (Input.GetButtonDown(VoxInput.rightShoulder))
			{
				clickDown = true;
				//interactionMode = VoxInteractionMode.AddBlocks;
				AddMode = true;
			}

			if (Input.GetButtonDown(VoxInput.leftShoulder))
			{
				clickDown = true;
                //interactionMode = VoxInteractionMode.SubtractBlocks;

                AddMode = false;
			}
			if (VoxInput.GetSelectDown())
			{
				clickDown = true;
			}

		    if (mouseLeftRightSwitchesAddAndSubtract)
		    {
		        if (Input.GetMouseButtonDown(0))
		        {
		            clickDown = true;
		            //interactionMode = VoxInteractionMode.SubtractBlocks;
		            AddMode = true;
		        }
		        if (Input.GetMouseButtonDown(1))
		        {
		            clickDown = true;
		            //interactionMode = VoxInteractionMode.SubtractBlocks;
		            AddMode = false;
		        }
		    }

		    if (Input.GetMouseButton(1))
			{
				click = true;
			}

			if (Input.GetMouseButtonUp(1))
			{
				clickUp = true;
			}

			if (Input.GetButton(VoxInput.rightShoulder)) click = true;
			if (Input.GetButton(VoxInput.leftShoulder)) click = true;
			if (VoxInput.GetSelect()) click = true;

			if (Input.GetButtonUp(VoxInput.rightShoulder)) clickUp = true;
			if (Input.GetButtonUp(VoxInput.leftShoulder)) clickUp = true;
			if (VoxInput.GetSelectUp()) clickUp = true;

			if (clickUp) click = false;

			var doubleClick = false;

			if (clickDown && doubleClickTimer <= doubleClickDelay)
			{
				doubleClick = true;
			}
			else if (clickDown)
			{
				doubleClickTimer = 0;
			}

			doubleClickTimer += Time.deltaTime;

			//if (Input.GetKeyDown(KeyCode.X))
			//{
			//	interactionMode++;

			//	if ((int) interactionMode >= Enum.GetValues(typeof (VoxInteractionMode)).Length)
			//	{
			//		interactionMode = 0;
			//	}
			//	//AddMode = !AddMode;
			//}

			var sampleColor = Input.GetButtonDown(VoxInput.ButtonB) || Input.GetKeyDown(KeyCode.C);

			if (Input.GetButtonDown(VoxInput.Select) || Input.GetKeyDown(KeyCode.P))
			{
				PaintButton.isSelected = !PaintButton.isSelected;

				if (PaintButton.isSelected)
				{
					interactionMode = VoxInteractionMode.Paint;

					PaintButton.isSelected = true;
					AddButton.isSelected = false;
					SubtractButton.isSelected = false;

					VoxManager.ShowMessage("Paint Mode Enabled");
				}
				else
				{
					interactionMode = VoxInteractionMode.Create;

					if (AddMode)
					{
						AddButton.isSelected = true;
						SubtractButton.isSelected = false;
					}
					else
					{
						AddButton.isSelected = false;
						SubtractButton.isSelected = true;
					}

					VoxManager.ShowMessage("Create Mode Enabled");
				}
			}

			if (Input.GetButtonDown(VoxInput.ButtonX) || Input.GetKeyDown(KeyCode.T))
			{
				DrawStraightButton.isSelected = !DrawStraightButton.isSelected;

				drawStraight = DrawStraightButton.isSelected;

				if (drawStraight)
				{
					secondBlockDrawn = false;
					startBlockPos = lastBlockPos;
				}

				VoxManager.ShowMessage(drawStraight ? "Draw Straight Mode Enabled" : "Draw Straight Mode Disabled");
			}


			if (Input.GetButtonDown(VoxInput.Start) || Input.GetKeyDown(KeyCode.U))
			{
				CreateAtFixedDistanceButton.isSelected = !CreateAtFixedDistanceButton.isSelected;

				createAtFixedDistance = CreateAtFixedDistanceButton.isSelected;

				if (createAtFixedDistance)
				{
					fixedDistance = Vector3.Distance(VoxManager.ViewPosition, VoxTerrain.GetUnityPosition(lastBlockPos));
				}
				else
				{
					//var bpu = VoxTerrain.GetUnityPosition(lastBlockPos);
					//var tobpu = viewPos - bpu;

					//constructionPlane = new Plane(viewForward, bpu);
					drawStarted = false;

					if(interactionMode == VoxInteractionMode.Create) clickDown = true;
				}

				VoxManager.ShowMessage(createAtFixedDistance ? "Draw At Fixed Distance Mode Enabled" : "Draw At Fixed Distance Mode Disabled");
			}

			if (Input.GetButtonDown(VoxInput.ButtonY) || Input.GetKeyDown(KeyCode.Y))
			{
				switch (interactionMode)
				{
					case VoxInteractionMode.Create:
						DoubleClickLayerButton.isSelected = !DoubleClickLayerButton.isSelected;

						doubleClickToIncreaseHeight = DoubleClickLayerButton.isSelected;

						VoxManager.ShowMessage(doubleClickToIncreaseHeight ? "Match Height Mode Enabled" : "Match Height Mode Disabled");
						break;
					case VoxInteractionMode.Paint:

						PaintUsingPlanesButton.isSelected = !PaintUsingPlanesButton.isSelected;

						paintUsingConstructionPlanes = PaintUsingPlanesButton.isSelected;

						VoxManager.ShowMessage(paintUsingConstructionPlanes ? "Paint Using Construction Planes Enabled" : "Paint Using Construction Planes  Disabled");
						break;
				}
			}

			var dpadX = Input.GetAxis(VoxInput.DPadX);

			if (Input.GetKeyDown(KeyCode.X)) dpadX = 1;
			if (Input.GetKeyDown(KeyCode.Z)) dpadX = -1;


			if (dpadX <= -0.95f && lastDPadX > -0.95f)
			{
				PrefabIndex--;
				if (PrefabIndex < 0) PrefabIndex = Prefabs.Length - 1;

				VoxManager.ShowMessage("Brush " + (PrefabIndex + 1).ToString("D"));
			}
			else if (dpadX >= 0.95f && lastDPadX < 0.95f)
			{
				PrefabIndex++;
				if (PrefabIndex >= Prefabs.Length) PrefabIndex = 0;
				VoxManager.ShowMessage("Brush " + (PrefabIndex + 1).ToString("D"));
			}

			lastDPadX = dpadX;

			Crosshair.SetCrosshairMode(AddMode);

			//Crosshair.mode = AddMode ? VoxCrosshair.CrosshairMode.Add : VoxCrosshair.CrosshairMode.Subtract;

			if (!drawStarted) // No drawing is happening, we need to create a cube by first clicking, then the draw starts until we release button
			{
				drawSecondLayer = false;
				inverseAdd = false;
				secondBlockDrawn = false;

				if (Physics.Raycast(ray, out hit, RayCastDistance, Layers.value)) // Looking at a block
				{
					//Debug.Log("Raycasting");
					var chunk = hit.collider.GetComponent<Chunk>();
					if (chunk != null)
					{
						var distance = hit.distance;
						//Debug.Log("Block hit");

						var blockPos = VoxTerrain.GetBlockPos(hit);
						var adjacentPos = VoxTerrain.GetBlockPos(hit, true);

						var blockPosUnity = VoxTerrain.GetUnityPosition(blockPos);
						var adjacentPosUnity = VoxTerrain.GetUnityPosition(adjacentPos);

						var facePosUnity = blockPosUnity + hit.normal*World.HalfUnitScale;

						if (sampleColor)
						{
							var block = chunk.World.GetBlock(VoxTerrain.GetBlockPos(hit));
							NewBlockColor = block.BlockColor;
							//VoxManager.ShowMessage("Color sampled");
						}

						// Place the crosshair on the face
						Crosshair.newPosition = facePosUnity;
						Crosshair.transform.forward = hit.normal;

						if (distance >= minBuildDistance)
						{

							switch (interactionMode)
							{
								case VoxInteractionMode.Create:
									if (clickDown) // Just pressed
									{
										if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;
										var prefab = Prefabs[PrefabIndex];
										var positions = prefab.GetWorldPositions();

										if (AddMode) // Add
										{
											PlayClip(sndCreate, true);
											if (doubleClickToIncreaseHeight)
											{
												var buildPos = doubleClick ? blockPos + hit.normal : blockPos;

												//lastBlockPos = buildPos;

												startBlockPos = buildPos;

												foreach (var pos in positions)
												{
													var newPos = buildPos + pos;
													//VoxWorld.SetBlock(newPos, new Block(NewBlockColor), true);

													VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
												}

												constructionBlockPos = blockPosUnity + hit.normal*World.UnitScale - hit.normal*World.HalfUnitScale;

												if (doubleClick)
												{
													drawSecondLayer = true;

													//constructionBlockPos = constructionBlockPos + constructionPlane.normal * World.UnitScale;
													//constructionPlane = new Plane(constructionPlane.normal, constructionBlockPos);
												}
												else
												{
													//constructionPlane = new Plane(hit.normal, constructionBlockPos);
												}

												constructionPlane = new Plane(hit.normal, constructionBlockPos);

												startBlockPos = buildPos;

												
											}
											else
											{
												var buildPos = adjacentPos;
												startBlockPos = buildPos;

												if (doubleClick)
												{
												}
												else
												{
													//lastBlockPos = buildPos;

													foreach (var pos in positions)
													{
														var newPos = buildPos + pos;
														VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
													}

													constructionBlockPos = adjacentPosUnity - hit.normal*World.HalfUnitScale;
													constructionPlane = new Plane(hit.normal, constructionBlockPos);
												}
											}
										}
										else // Subtract
										{
											PlayClip(sndDelete, true);

											var buildPos = blockPos;

											startBlockPos = buildPos;

											foreach (var pos in positions)
											{
												var newPos = buildPos + pos;
												VoxWorld.SetBlock(newPos, VoxBlockType.Empty, NewBlockColor, true);
											}

											constructionBlockPos = adjacentPosUnity - hit.normal*World.HalfUnitScale;
											constructionPlane = new Plane(hit.normal, constructionBlockPos);
										}
										adjacentPlane = new Plane(hit.normal, hit.point);
										drawStarted = true;
										blocksModified = true;

										
									}
									break;
								case VoxInteractionMode.Paint:
									if (clickDown)
									{
										PlayClip(sndPaint, true);

										if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;
										var prefab = Prefabs[PrefabIndex];
										var buildPos = blockPos;

										var positions = prefab.GetWorldPositions();

										startBlockPos = buildPos;

										foreach (var pos in positions)
										{
											var newPos = buildPos + pos;

											var block = VoxWorld.GetBlock(newPos);

											if (!block.IsEmpty())
											{
												VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, false);

												//block.BlockColor = NewBlockColor;
												//block.Modified = true;
												//VoxWorld.GetChunk(newPos.x, newPos.y, newPos.z, false).RebuildNeeded = true;
											}
										}

										constructionPlane = new Plane(hit.normal, blockPosUnity + hit.normal*World.HalfUnitScale);
										adjacentPlane = new Plane(hit.normal, hit.point);
										drawStarted = true;
									}
									break;
								case VoxInteractionMode.Teleport:
									if (clickDown)
									{
										PlayClip(sndTeleport, true);

										VoxManager.TeleportPlayer(blockPosUnity);
										constructionPlane = new Plane(hit.normal, adjacentPosUnity - hit.normal*World.HalfUnitScale);
										adjacentPlane = new Plane(hit.normal, hit.point);
									}

									break;
							}
						}

						
					}
				}
				else // Not looking at anything
				{
					//Debug.Log("Not Raycasting");
					// Raycast against the construction plane
					float enter = 0f;
					var posOnPlaneUnity = Vector3.zero;
					if (constructionPlane.Raycast(ray, out enter))
					{
						posOnPlaneUnity = viewPos + viewForward*enter;
					}

					//if (adjacentPlane.Raycast(ray, out enter))
					//{
					//	posOnPlaneUnity = viewPos + viewForward * enter;

					//	var sign = constructionPlane.GetSide(posOnPlaneUnity) ? 1 : -1;

					//	posOnPlaneUnity += constructionPlane.normal * constructionPlane.GetDistanceToPoint(posOnPlaneUnity) * sign;
					//}

					var blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity + constructionPlane.normal * World.HalfUnitScale);

					var blockPosUnity = VoxTerrain.GetUnityPosition(blockPos);

					var facePosUnity = blockPosUnity - constructionPlane.normal*World.HalfUnitScale;

					Crosshair.newPosition = facePosUnity;
					Crosshair.transform.forward = constructionPlane.normal;

					if (enter >= minBuildDistance)
					{

						switch (interactionMode)
						{
							case VoxInteractionMode.Create:
								if (clickDown) // Create a block in air that is built on the construction plane
								{
									if (AddMode) // Add 
									{
										if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;
										var prefab = Prefabs[PrefabIndex];

										PlayClip(sndCreate, true);

										var positions = prefab.GetWorldPositions();


										blockPos = doubleClickToIncreaseHeight ? VoxTerrain.GetBlockPos(posOnPlaneUnity - constructionPlane.normal*World.HalfUnitScale) : VoxTerrain.GetBlockPos(posOnPlaneUnity + constructionPlane.normal*World.HalfUnitScale);

										var buildPos = blockPos;

										startBlockPos = buildPos;

										//if (doubleClickToIncreaseHeight) buildPos = buildPos - hit.normal * 2;

										//lastBlockPos = buildPos;

										foreach (var pos in positions)
										{
											var newPos = buildPos + pos;
											VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
										}

										adjacentPlane = constructionPlane;
									}
									else // Subtract
									{
										if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;
										var prefab = Prefabs[PrefabIndex];

										PlayClip(sndDelete, true);

										inverseAdd = true; // Draw opposite of construction plane

										var positions = prefab.GetWorldPositions();

										//var buildPos = doubleClick ? blockPos + hit.normal : blockPos;

										var buildPos = VoxTerrain.GetBlockPos(posOnPlaneUnity - constructionPlane.normal*World.HalfUnitScale);
										//VoxWorld.SetBlock(oppositeBlockPos, new Block(NewBlockColor), true);
										startBlockPos = buildPos;

										//lastBlockPos = buildPos;

										foreach (var pos in positions)
										{
											var newPos = buildPos + pos;
											VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
										}

										//constructionPlane = new Plane(constructionPlane.normal, VoxTerrain.GetUnityPosition(buildPos));
										adjacentPlane = constructionPlane;



										//if (UsePrefab)
										//{

										//}
										//else
										//{


										//}
									}
									drawStarted = true;
									blocksModified = true;

								}
								break;
							case VoxInteractionMode.Paint:
								break;
						}
					}
				}


				if (createAtFixedDistance)
				{
					fixedDistance = Vector3.Distance(VoxManager.ViewPosition, VoxTerrain.GetUnityPosition(startBlockPos));
				}

				if (drawStarted)
				{
					lastBlockPos = startBlockPos;

					DrawTimer = ActivateDrawTime;
				}
				//drawStarted = false; // stop draw when the click is released

			}
			else // Now we're drawing, new cubes are always added on construction plane, so we'll only raycast the construction plane
			{
				if (!click) drawStarted = false; // stop draw when the click is released

				if (DrawTimer < 0)
				{

					float enter = 0f;
					var posOnPlaneUnity = Vector3.zero;

					WorldPos blockPos;
					Vector3 blockPosUnity;
					Vector3 facePosUnity;

					switch (interactionMode)
					{
						case VoxInteractionMode.Create:
							if (createAtFixedDistance)
							{
								if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;

								var prefab = Prefabs[PrefabIndex];

								blockPosUnity = viewPos + viewForward * fixedDistance;

								blockPos = VoxTerrain.GetBlockPos(blockPosUnity);

								var positions = prefab.GetWorldPositions();

								if (!blockPos.Equals(lastBlockPos))
								{
									foreach (var pos in positions)
									{
										var newPos = blockPos + pos;
										if (AddMode)
										{
											VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
										}
										else
										{
											VoxWorld.SetBlock(newPos, VoxBlockType.Empty, NewBlockColor, true);
										}
									}
									PlayClip(sndCreateDrag);
								}

								lastBlockPos = blockPos;
							}
							else
							{
								if (AddMode)
								{
									if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;

									var prefab = Prefabs[PrefabIndex];

									if (doubleClickToIncreaseHeight)
									{
										if (constructionPlane.Raycast(ray, out enter))
										{
											posOnPlaneUnity = viewPos + viewForward*enter;

											if (!drawSecondLayer) posOnPlaneUnity -= constructionPlane.normal*World.UnitScale;
										}
									}
									else
									{
										if (adjacentPlane.Raycast(ray, out enter))
										{
											posOnPlaneUnity = viewPos + viewForward*enter;

											var sign = constructionPlane.GetSide(posOnPlaneUnity) ? 1 : -1;

											posOnPlaneUnity += constructionPlane.normal*constructionPlane.GetDistanceToPoint(posOnPlaneUnity)*sign;
										}
									}

									//if (adjacentPlane.Raycast(ray, out enter))
									//{
									//	posOnPlaneUnity = viewPos + viewForward * enter;

									//	var sign = constructionPlane.GetSide(posOnPlaneUnity) ? 1 : -1;

									//	posOnPlaneUnity += constructionPlane.normal * constructionPlane.GetDistanceToPoint(posOnPlaneUnity) * sign;
									//}




									//if (constructionPlane.Raycast(ray, out enter))
									//{
									//	posOnPlaneUnity = viewPos + viewForward * enter;

									//	//var sign = constructionPlane.GetSide(posOnPlaneUnity) ? 1 : -1;

									//	//posOnPlaneUnity += constructionPlane.normal * constructionPlane.GetDistanceToPoint(posOnPlaneUnity) * sign;

									//	//if (constructionPlane.Raycast(new Ray(posOnPlaneUnity, adjacentPlane.normal), out enter))
									//	//{
									//	//	posOnPlaneUnity = viewPos + viewForward * enter;
									//	//}
									//}

									//if (doubleClickToIncreaseHeight)
									//{
									//	blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity - constructionPlane.normal * World.HalfUnitScale - constructionPlane.normal * World.UnitScale);
									//}
									//else
									//{
									//	blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity + constructionPlane.normal * World.HalfUnitScale);
									//}

									blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity + constructionPlane.normal*World.HalfUnitScale);

									blockPosUnity = VoxTerrain.GetUnityPosition(blockPos);

									if (drawStraight)
									{
										if (!secondBlockDrawn)
										{
											if (!blockPos.Equals(startBlockPos))
											{
												secondBlockPos = blockPos;
												secondBlockDrawn = true;

												var drawNormal = secondBlockPos.vector - startBlockPos.vector;

												if (Mathf.Abs(Vector3.Dot(drawNormal, Vector3.up)) > 0.7f)
												{
													secondBlockPos.x = startBlockPos.x;
													secondBlockPos.z = startBlockPos.z;
												}
												else if (Mathf.Abs(Vector3.Dot(drawNormal, Vector3.forward)) > 0.7f)
												{
													secondBlockPos.x = startBlockPos.x;
													secondBlockPos.y = startBlockPos.y;
												}
												else if (Mathf.Abs(Vector3.Dot(drawNormal, Vector3.right)) > 0.7f)
												{
													secondBlockPos.y = startBlockPos.y;
													secondBlockPos.z = startBlockPos.z;
												}
											}
										}

										if (secondBlockDrawn)
										{
											var toNewBlock = blockPos.vector - startBlockPos.vector;
											var drawNormal = secondBlockPos.vector - startBlockPos.vector;

											var p = Vector3.Project(toNewBlock, drawNormal);

											blockPos = startBlockPos + p;
										}
									}

									facePosUnity = blockPosUnity - constructionPlane.normal*World.HalfUnitScale;

									Crosshair.newPosition = facePosUnity;

									var positions = prefab.GetWorldPositions();

									if (!blockPos.Equals(lastBlockPos))
									{
										foreach (var pos in positions)
										{
											var newPos = blockPos + pos;
											VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
										}
										PlayClip(sndCreateDrag);
									}

									lastBlockPos = blockPos;
								}
								else // Subtract mode
								{
									if (constructionPlane.Raycast(ray, out enter))
									{
										posOnPlaneUnity = viewPos + viewForward*enter - constructionPlane.normal*World.UnitScale;
									}

									//if (inverseAdd) blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity - constructionPlane.normal * World.HalfUnitScale);
									//else 

									blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity + constructionPlane.normal*World.HalfUnitScale);

									blockPosUnity = VoxTerrain.GetUnityPosition(blockPos);

									if (drawStraight)
									{
										if (!secondBlockDrawn)
										{
											if (!blockPos.Equals(startBlockPos))
											{
												secondBlockPos = blockPos;
												secondBlockDrawn = true;

												var drawNormal = secondBlockPos.vector - startBlockPos.vector;

												if (Mathf.Abs(Vector3.Dot(drawNormal, Vector3.up)) > 0.7f)
												{
													secondBlockPos.x = startBlockPos.x;
													secondBlockPos.z = startBlockPos.z;
												}
												else if (Mathf.Abs(Vector3.Dot(drawNormal, Vector3.forward)) > 0.7f)
												{
													secondBlockPos.x = startBlockPos.x;
													secondBlockPos.y = startBlockPos.y;
												}
												else if (Mathf.Abs(Vector3.Dot(drawNormal, Vector3.right)) > 0.7f)
												{
													secondBlockPos.y = startBlockPos.y;
													secondBlockPos.z = startBlockPos.z;
												}
											}
										}

										if (secondBlockDrawn)
										{
											var toNewBlock = blockPos.vector - startBlockPos.vector;
											var drawNormal = secondBlockPos.vector - startBlockPos.vector;

											var p = Vector3.Project(toNewBlock, drawNormal);

											blockPos = startBlockPos + p;
										}
									}


									facePosUnity = blockPosUnity - constructionPlane.normal*World.HalfUnitScale;

									Crosshair.newPosition = facePosUnity;

									if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;

									var prefab = Prefabs[PrefabIndex];

									var positions = prefab.GetWorldPositions();

									if (!lastBlockPos.Equals(blockPos))
									{
										foreach (var pos in positions)
										{
											var newPos = blockPos + pos;

											if (inverseAdd) VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, true);
											else VoxWorld.SetBlock(newPos, VoxBlockType.Empty, NewBlockColor, true);
										}
										PlayClip(sndDeleteDrag);

									}
									lastBlockPos = blockPos;
								}
							}
							blocksModified = true;
							break;
						case VoxInteractionMode.Paint:

							if (paintUsingConstructionPlanes)
							{
								if (constructionPlane.Raycast(ray, out enter))
								{
									posOnPlaneUnity = viewPos + viewForward*enter;
								}

								blockPos = VoxTerrain.GetBlockPos(posOnPlaneUnity - constructionPlane.normal*World.HalfUnitScale);

								//blockPosUnity = VoxTerrain.GetUnityPosition(blockPos);

								//facePosUnity = blockPosUnity + constructionPlane.normal*World.HalfUnitScale;

								Crosshair.newPosition = posOnPlaneUnity;

								if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;

								var prefab = Prefabs[PrefabIndex];

								var positions = prefab.GetWorldPositions();

								if (!lastBlockPos.Equals(blockPos))
								{
									foreach (var pos in positions)
									{
										var newPos = blockPos + pos;
										var block = VoxWorld.GetBlock(newPos);
										if (!block.IsEmpty())
										{
											//block.BlockColor = NewBlockColor;
											//block.Modified = true;
											//VoxWorld.GetChunk(newPos.x, newPos.y, newPos.z, false).RebuildNeeded = true;

											VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, false);

										}
									}
									PlayClip(sndPaintDrag);
								}
								lastBlockPos = blockPos;
							}
							else
							{
								if (Physics.Raycast(ray, out hit, RayCastDistance, Layers.value)) // Looking at a block
								{
									var chunk = hit.collider.GetComponent<Chunk>();
									if (chunk != null)
									{
										blockPos = VoxTerrain.GetBlockPos(hit);

										if (PrefabIndex >= Prefabs.Length || PrefabIndex < 0) break;

										var prefab = Prefabs[PrefabIndex];

										var positions = prefab.GetWorldPositions();

										if (!lastBlockPos.Equals(blockPos))
										{
											foreach (var pos in positions)
											{
												var newPos = blockPos + pos;
												var block = VoxWorld.GetBlock(newPos);
												if (!block.IsEmpty())
												{
													//block.BlockColor = NewBlockColor;
													//block.Modified = true;
													//VoxWorld.GetChunk(newPos.x, newPos.y, newPos.z, false).RebuildNeeded = true;

													VoxWorld.SetBlock(newPos, VoxBlockType.Default, NewBlockColor, false);

												}
											}
											PlayClip(sndPaintDrag);
										}
										lastBlockPos = blockPos;
									}
								}
							}
							break;
					}
				}
			}

			DrawTimer -= Time.deltaTime;


			if (blocksModified)
			{

				//var playerPos = VoxTerrain.GetBlockPos(VoxManager.Player.position);

				//for (var px = -3; px < 3; px++)
				//{
				//	for (var py = -3; py < 5; py++)
				//	{
				//		for (var pz = -3; pz < 3; pz++)
				//		{
				//			var p = playerPos + new WorldPos(px, py, pz);

				//			var b = VoxWorld.GetBlock(p);

				//			if (b.JustCreated && !b.IsEmpty()) VoxWorld.SetBlock(p, VoxBlockType.Empty, Color.white);
				//		}
				//	}
				//}
			}
		}

		protected void Update()
		{

			if (MainMenu.IsVisible || !VoxManager.InitialBuildFinished)
			{
				Crosshair.SetVisible(false);
				//Crosshair.newPosition = VoxManager.ViewPosition - VoxManager.ViewForward*100f;
				//Crosshair.transform.position = Crosshair.newPosition;
				return;
			}

			switch (interactionMode)
			{
				case VoxInteractionMode.Create:
					Crosshair.SetVisible(true);
					break;
				case VoxInteractionMode.Paint:
					Crosshair.SetVisible(paintUsingConstructionPlanes);
					break;
				case VoxInteractionMode.Teleport:
					Crosshair.SetVisible(true);
					break;
			}

			Crosshair.SetColor(NewBlockColor);

			InterfaceWithBlocks();
			audioClipTimer += Time.deltaTime;

			//return;


			//RaycastHit hit;

			//var cameraPosition = VoxManager.ViewPosition;
			//var cameraForward = VoxManager.ViewForward;

			//var ray = VoxManager.ViewRay;

			//createTimer = 0;

			//var select = Input.GetButton(VoxInput.selectButton);
			//var mouseX = Input.GetAxis(VoxInput.MouseX);
			//var mouseY = Input.GetAxis(VoxInput.MouseY);

			//var modifyBlock = false;

			//if (Input.GetKeyDown(KeyCode.Escape))
			//{
			//	//AddMode = !AddMode;
			//}
			//if (Input.GetButtonDown(VoxInput.rightShoulder))
			//{
			//	modifyBlock = true;
			//	AddMode = true;
			//}
			//if (Input.GetButtonDown(VoxInput.leftShoulder))
			//{
			//	modifyBlock = true;
			//	AddMode = false;
			//}
			//if (!Crosshair.CanDoubleClickToOpenMenu && VoxInput.GetSelectDown())
			//{
			//	modifyBlock = true;
			//}


			//var click = VoxInput.GetSelectDown();

			//var clickDown = VoxInput.GetSelect();
			//var clickHeld = lastClickDown == clickDown;
			//lastClickDown = clickDown;


			

			//var doubleClick = false;

			//if (doubleClickTimer <= doubleClickDelay)
			//{
			//	if (click)
			//	{
			//		doubleClick = true;
			//		checkModify = false;
			//		modifyBlock = false;
			//	}
			//}
			//else
			//{
			//	if (checkModify && !clickDown)
			//	{
			//		checkModify = false;
			//		modifyBlock = true;
			//	}
			//	else
			//	{
			//		if (click)
			//		{
			//			doubleClickTimer = 0;
			//			clickHeldTimer = 0;
			//			checkModify = true;
			//		}
			//	}
			//}

			//if (clickDown)
			//{
			//	//doubleClickTimer = 0;
			//	if (clickHeldTimer > doubleClickDelay) checkModify = false;
			//	clickHeldTimer += Time.deltaTime;
			//}



			//else if (checkModify)
			//{
			//	if(!clickHeld) modifyBlock = true;
			//	checkModify = false;

			//}
			//else if (click)
			//{
			//	doubleClickTimer = 0f;
			//	checkModify = true;
			//}

			//doubleClickTimer += Time.deltaTime;


			//if (Input.GetButtonDown(VoxInput.selectButton))
			//{
			//	modifyBlock = true;
			//}

			//if (modifyBlock)
			//{
			//	if (Physics.Raycast(ray, out hit, RayCastDistance))
			//	{

			//		if (AddMode)
			//		{
			//			if (Vector3.Distance(cameraPosition, hit.point) >= World.UnitScale*2)
			//			{
			//				startBlockPos = VoxTerrain.GetBlockPos(hit, true);
			//				lastBlockPos = startBlockPos;
			//				startRaycastPos = VoxTerrain.GetUnityPosition(startBlockPos);

			//				constructionPlane = new Plane(hit.normal, hit.point + hit.normal * VoxWorld.VisualScale * 0.5f);


			//				//Crosshair.lockToPlane = true;
			//				//constructionPlane = GetXYZPlaneFacingDirection(cameraForward, startRaycastPos);


			//				//NewBlockColor = World.GetRandomColor();
			//				VoxTerrain.SetBlock(hit, new Block(NewBlockColor), true);

			//				DrawTimer = 0;
			//			}
			//		}
			//		else
			//		{
			//			startBlockPos = VoxTerrain.GetBlockPos(hit);
			//			lastBlockPos = startBlockPos;
			//			startRaycastPos = VoxTerrain.GetUnityPosition(startBlockPos);
			//			constructionPlane = new Plane(hit.normal, hit.point - hit.normal * VoxWorld.VisualScale * 0.5f);

			//			//constructionPlane = GetXYZPlaneFacingDirection(cameraForward, startRaycastPos);

			//			//Crosshair.ConstructionPlane = constructionPlane;

			//			VoxTerrain.SetBlock(hit, new BlockEmpty());

			//			DrawTimer = 0;
			//		}

			//		Crosshair.ConstructionPlane = constructionPlane;
			//	}
			//}
			//else
			//{
			//	//Crosshair.transform.forward
			//}

			//modifyBlock = false;


			//if (Input.GetButton(VoxInput.rightShoulder))
			//{
			//	modifyBlock = true;
			//	AddMode = true;
			//}
			//if (Input.GetButton(VoxInput.leftShoulder))
			//{
			//	modifyBlock = true;
			//	AddMode = false;
			//}
			//if (!Crosshair.CanDoubleClickToOpenMenu && Input.GetButton(VoxInput.selectButton))
			//{
			//	modifyBlock = true;
			//}

			////if (modifyBlock)
			//if (modifyBlock && DrawTimer >= ActivateDrawTime)
			//{
			//	//Crosshair.lockToPlane = true;
			//	//Crosshair.ConstructionPlane = constructionPlane;

			//	float enter;
			//	if (constructionPlane.Raycast(ray, out enter))
			//	{
			//		if (enter <= RayCastDistance)
			//		{
			//			var pointOnPlane = ray.origin + ray.direction*enter;

			//			//Crosshair.newPosition = pointOnPlane;
			//			//Crosshair.transform.forward = constructionPlane.normal;

			//			var blockPos = VoxTerrain.GetBlockPos(pointOnPlane);

			//			if (!blockPos.Equals(lastBlockPos))
			//			{
			//				if (AddMode)
			//				{
			//					VoxWorld.SetBlock(blockPos.x, blockPos.y, blockPos.z, new Block(NewBlockColor), true);
			//				}
			//				else
			//				{
			//					VoxWorld.SetBlock(blockPos.x, blockPos.y, blockPos.z, new BlockEmpty());
			//				}
			//			}

			//			lastBlockPos = blockPos;
			//		}
			//	}
			//}
			//else
			//{
			//	Crosshair.lockToPlane = false;
			//}

			//if (modifyBlock)
			//{
			//	DrawTimer += Time.deltaTime;
			//}

			//Crosshair.mode = AddMode ? VoxCrosshair.CrosshairMode.Add : VoxCrosshair.CrosshairMode.Subtract;
		}
	}
}