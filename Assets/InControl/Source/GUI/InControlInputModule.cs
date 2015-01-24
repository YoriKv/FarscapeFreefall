#if UNITY_4_6
using UnityEngine;
using UnityEngine.EventSystems;
using InControl;


namespace InControl
{
	[AddComponentMenu( "Event/InControl Input Module" )]
	public class InControlInputModule : PointerInputModule
	{
		public enum Button : int
		{
			Action1 = InputControlType.Action1,
			Action2 = InputControlType.Action2,
			Action3 = InputControlType.Action3,
			Action4 = InputControlType.Action4
		}


		private enum InputSource : int
		{
			InControl,
			Mouse
		}


		public Button submitButton = Button.Action1;
		public Button cancelButton = Button.Action2;
		[Range( 0.1f, 0.9f )]
		public float analogMoveThreshold = 0.5f;
		public float moveRepeatFirstDuration = 0.8f;
		public float moveRepeatDelayDuration = 0.1f;
		public bool allowMobileDevice = true;
		public bool allowMouseInput = true;
		public bool focusOnMouseHover = true;

		InputDevice inputDevice;
		InputSource currentInputSource;
		Vector3 thisMousePosition;
		Vector3 lastMousePosition;
		Vector2 thisVectorState;
		Vector2 lastVectorState;
		bool thisSubmitState;
		bool lastSubmitState;
		bool thisCancelState;
		bool lastCancelState;
		float nextMoveRepeatTime;
		float lastVectorPressedTime;


		protected InControlInputModule()
		{
			TwoAxisInputControl.StateThreshold = analogMoveThreshold;
			currentInputSource = InputSource.InControl;
		}


		public override bool IsModuleSupported()
		{
			return allowMobileDevice || !Application.isMobilePlatform;
		}


		public override bool ShouldActivateModule()
		{
			if (!base.ShouldActivateModule())
			{
				return false;
			}

			UpdateInputState();

			var shouldActivate = false;
			shouldActivate |= SubmitWasPressed;
			shouldActivate |= CancelWasPressed;
			shouldActivate |= VectorWasPressed;

			if (allowMouseInput)
			{
				shouldActivate |= MouseHasMoved();
				shouldActivate |= MouseButtonIsPressed();
			}

			return shouldActivate;
		}


		void UpdateInputState()
		{
			lastVectorState = thisVectorState;
			thisVectorState = Vector2.zero;

			if (Mathf.Abs( Direction.X ) > analogMoveThreshold)
			{
				thisVectorState.x = Mathf.Sign( Direction.X );
			}

			if (Mathf.Abs( Direction.Y ) > analogMoveThreshold)
			{
				thisVectorState.y = Mathf.Sign( Direction.Y );
			}

			if (VectorIsReleased)
			{
				nextMoveRepeatTime = 0.0f;
			}

			if (VectorIsPressed)
			{
				if (lastVectorState == Vector2.zero)
				{
					if (Time.realtimeSinceStartup > lastVectorPressedTime + 0.1f)
					{
						nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatFirstDuration;
					}
					else
					{
						nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatDelayDuration;
					}
				}

				lastVectorPressedTime = Time.realtimeSinceStartup;
			}

			lastSubmitState = thisSubmitState;
			thisSubmitState = SubmitButton.IsPressed;

			lastCancelState = thisCancelState;
			thisCancelState = CancelButton.IsPressed;
		}


		void SetVectorRepeatTimer()
		{
			nextMoveRepeatTime = Mathf.Max( nextMoveRepeatTime, Time.realtimeSinceStartup + moveRepeatDelayDuration );
		}


		bool VectorIsPressed
		{
			get
			{
				return thisVectorState != Vector2.zero;
			}
		}


		bool VectorIsReleased
		{
			get
			{
				return thisVectorState == Vector2.zero;
			}
		}


		bool VectorHasChanged
		{
			get
			{
				return thisVectorState != lastVectorState;
			}
		}


		bool VectorWasPressed
		{
			get
			{
				if (VectorIsPressed && Time.realtimeSinceStartup > nextMoveRepeatTime)
				{
					return true;
				}

				return VectorIsPressed && lastVectorState == Vector2.zero;
			}
		}


		bool SubmitWasPressed
		{
			get
			{
				return thisSubmitState && thisSubmitState != lastSubmitState;
			}
		}


		bool CancelWasPressed
		{
			get
			{
				return thisCancelState && thisCancelState != lastCancelState;
			}
		}


		public override void ActivateModule()
		{
			base.ActivateModule();

			thisMousePosition = Input.mousePosition;
			lastMousePosition = Input.mousePosition;

			var baseEventData = GetBaseEventData();
			var gameObject = eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = eventSystem.lastSelectedGameObject;
			}
			if (gameObject == null)
			{
				gameObject = eventSystem.firstSelectedGameObject;
			}
			eventSystem.SetSelectedGameObject( null, baseEventData );
			eventSystem.SetSelectedGameObject( gameObject, baseEventData );
		}


		public override void DeactivateModule()
		{
			base.DeactivateModule();
			base.ClearSelection();
		}


		public override void Process()
		{
			var used = SendUpdateEventToSelectedObject();

			if (!used)
			{
				used = SendVectorEventToSelectedObject();
			}

			if (!used)
			{
				SendButtonEventToSelectedObject();
			}

			if (allowMouseInput)
			{
				ProcessMouseEvent();
			}
		}


		/*
		void OnGUI()
		{
			var item = eventSystem.currentSelectedGameObject;
			var name = (item == null) ? "(null)" : item.name;
			GUI.Label( new Rect( 10, 10, 200, 30 ), name );
		}
		*/


		bool SendButtonEventToSelectedObject()
		{
			if (eventSystem.currentSelectedGameObject == null /*|| currentInputSource != InputSource.InControl*/)
			{
				return false;
			}

			var baseEventData = GetBaseEventData();

			if (SubmitWasPressed)
			{
				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler );
			}

			if (CancelWasPressed)
			{
				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler );
			}

			return baseEventData.used;
		}


		bool SendVectorEventToSelectedObject()
		{
			if (!VectorWasPressed)
			{
				return false;
			}

			var axisEventData = GetAxisEventData( thisVectorState.x, thisVectorState.y, 0.5f );

			if (axisEventData.moveDir != MoveDirection.None)
			{
				if (currentInputSource != InputSource.InControl)
				{
					currentInputSource = InputSource.InControl;
					if (ResetSelection())
					{
						return true;
					}
				}

				ExecuteEvents.Execute( eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler );

				SetVectorRepeatTimer();
			}

			return axisEventData.used;
		}


		void ProcessMouseEvent()
		{
			var mousePointerEventData = this.GetMousePointerEventData();
			var pressed = mousePointerEventData.AnyPressesThisFrame();
			var released = mousePointerEventData.AnyReleasesThisFrame();
			var eventData = mousePointerEventData.GetButtonState( PointerEventData.InputButton.Left ).eventData;
			if (!UseMouse( pressed, released, eventData.buttonData ))
			{
				return;
			}
			ProcessMousePress( eventData );
			ProcessMove( eventData.buttonData );
			ProcessDrag( eventData.buttonData );
			ProcessMousePress( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Right ).eventData );
			ProcessDrag( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Right ).eventData.buttonData );
			ProcessMousePress( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Middle ).eventData );
			ProcessDrag( mousePointerEventData.GetButtonState( PointerEventData.InputButton.Middle ).eventData.buttonData );
			if (!Mathf.Approximately( eventData.buttonData.scrollDelta.sqrMagnitude, 0.0f ))
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>( eventData.buttonData.pointerCurrentRaycast.gameObject );
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>( eventHandler, eventData.buttonData, ExecuteEvents.scrollHandler );
			}
		}


		protected override void ProcessMove( PointerEventData pointerEvent )
		{
			base.ProcessMove( pointerEvent );

			if (focusOnMouseHover)
			{
				var eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>( pointerEvent.pointerEnter );
				eventSystem.SetSelectedGameObject( eventHandler, pointerEvent );
			}
		}


		private void ProcessMousePress( PointerInputModule.MouseButtonEventData data )
		{
			var buttonData = data.buttonData;
			var gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				var gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>( gameObject, buttonData, ExecuteEvents.pointerDownHandler );
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>( gameObject );
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					float num = unscaledTime - buttonData.clickTime;
					if (num < 0.3f)
					{
						buttonData.clickCount++;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>( gameObject );
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>( buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag );
				}
			}

			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>( buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler );
				var eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>( gameObject );
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>( buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler );
				}
				else
				{
					if (buttonData.pointerDrag != null)
					{
						ExecuteEvents.ExecuteHierarchy<IDropHandler>( gameObject, buttonData, ExecuteEvents.dropHandler );
					}
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				buttonData.dragging = false;
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IEndDragHandler>( buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler );
				}
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					base.HandlePointerExitAndEnter( buttonData, null );
					base.HandlePointerExitAndEnter( buttonData, gameObject );
				}
			}
		}


		bool ResetSelection()
		{
			var baseEventData = GetBaseEventData();
			var lastPointerEventData = base.GetLastPointerEventData( -1 );
			var rootGameObject = (lastPointerEventData != null) ? lastPointerEventData.pointerEnter : null;
			base.HandlePointerExitAndEnter( lastPointerEventData, null );
			base.eventSystem.SetSelectedGameObject( null, baseEventData );
			var result = false;
			var gameObject = ExecuteEvents.GetEventHandler<ISelectHandler>( rootGameObject );
			if (gameObject == null)
			{
				gameObject = eventSystem.lastSelectedGameObject;
				result = true;
			}
			base.eventSystem.SetSelectedGameObject( gameObject, baseEventData );
			return result;
		}


		bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			var baseEventData = GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>( base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler );
			return baseEventData.used;
		}


		public override void UpdateModule()
		{
			lastMousePosition = thisMousePosition;
			thisMousePosition = Input.mousePosition;
		}


		bool UseMouse( bool pressed, bool released, PointerEventData pointerData )
		{
			if (currentInputSource == InputSource.Mouse)
			{
				return true;
			}

			if (pressed || released || pointerData.IsPointerMoving() || pointerData.IsScrolling())
			{
				currentInputSource = InputSource.Mouse;
				base.eventSystem.SetSelectedGameObject( null, pointerData );
			}

			return currentInputSource == InputSource.Mouse;
		}


		bool MouseHasMoved()
		{
			return (thisMousePosition - lastMousePosition).sqrMagnitude > 0.0f;
		}


		bool MouseButtonIsPressed()
		{
			return Input.GetMouseButtonDown( 0 );
		}


		InputDevice Device
		{
			set
			{
				inputDevice = value;
			}

			get
			{
				return inputDevice ?? InputManager.ActiveDevice;
			}
		}


		TwoAxisInputControl Direction
		{
			get
			{
				return Device.Direction;
			}
		}


		InputControl SubmitButton
		{
			get
			{
				return Device.GetControl( (InputControlType) submitButton );
			}
		}


		InputControl CancelButton
		{
			get
			{
				return Device.GetControl( (InputControlType) cancelButton );
			}
		}
	}
}
#endif

