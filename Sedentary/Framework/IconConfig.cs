using System;
using System.Drawing;
using Sedentary.Model;

namespace Sedentary.Framework
{
	public struct IconConfig
	{
		private readonly WorkState _workState;
		private readonly Color _overlayColor;
		private readonly int _overlayHeight;

		public IconConfig(WorkState workState) : this (workState, Color.Transparent, 0)
		{}

		public IconConfig(WorkState workState, Color overlayColor, int overlayHeight)
		{
			_workState = workState;
			_overlayColor = overlayColor;
			_overlayHeight = (int) Math.Min(Math.Max(0, overlayHeight), 16);
		}

		public WorkState WorkState
		{
			get { return _workState; }
		}

		public Color OverlayColor
		{
			get { return _overlayColor; }
		}

		public int OverlayHeight
		{
			get { return _overlayHeight; }
		}

		public override int GetHashCode()
		{
			return _workState.GetHashCode() ^ _overlayColor.GetHashCode() ^ OverlayHeight.GetHashCode();
		}

		public IconConfig SetOverlay(Color color, int height)
		{
			return new IconConfig(WorkState, color, height);
		}

		public IconConfig SetWorkState(WorkState state)
		{
			return new IconConfig(state, OverlayColor, OverlayHeight);
		}
	}
}