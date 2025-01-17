﻿// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.InputSystem;

namespace Fungus
{
    /// <summary>
    /// Store Input.mousePosition and mouse screen conversions in a variable(s)
    /// </summary>
    [CommandInfo("Input",
                 "Get Mouse Position",
                 "Store various interpretations of Input.mousePosition")]
    [AddComponentMenu("")]
    public class GetMousePosition : Command
    {
        [VariableProperty(typeof(Vector2Variable))]
        [SerializeField]
        protected Vector2Variable screenPosition;

        [Tooltip("If null, Camera.main is used")]
        protected Camera castCamera;

        [VariableProperty(typeof(Vector2Variable))]
        [SerializeField]
        protected Vector2Variable viewPosition;

        [VariableProperty(typeof(Vector3Variable))]
        [SerializeField]
        protected Vector3Variable worldPosition;

        [VariableProperty(typeof(Vector3Variable))]
        [SerializeField]
        protected Vector3Variable worldDirection;

        public override void OnEnter()
        {
            if (castCamera == null)
            {
                castCamera = Camera.main;
            }

#if ENABLE_INPUT_SYSTEM
            var mousePos = UnityEngine.InputSystem.Mouse.current?.position.ReadValue() ?? Vector2.zero;
#else
            var mousePos = Input.mousePosition;
#endif

            if (screenPosition != null)
            {
                screenPosition.Value = mousePos;
            }

            if (viewPosition != null)
            {
                viewPosition.Value = castCamera.ScreenToViewportPoint(mousePos);
            }

            if (worldPosition != null)
            {
                Vector3 screenWithZ = mousePos;
                screenWithZ.z = castCamera.nearClipPlane;
                worldPosition.Value = castCamera.ScreenToWorldPoint(screenWithZ);
            }

            if (worldDirection != null)
            {
                Vector3 screenWithZ = mousePos;
                screenWithZ.z = castCamera.nearClipPlane;
                worldDirection.Value = castCamera.ScreenPointToRay(screenWithZ).direction;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return (screenPosition != null ? screenPosition.Key + " " : "") +
                    (castCamera != null ? castCamera.name + " " : "MainCam") +
                    (viewPosition != null ? viewPosition.Key + " " : "") +
                    (worldPosition != null ? worldPosition.Key + " " : "") +
                    (worldDirection != null ? worldDirection.Key + " " : "");
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return (screenPosition == variable ||
                viewPosition == variable ||
                worldPosition == variable ||
                worldDirection == variable);
        }
    }
}