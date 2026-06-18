using System;
using System.Collections.Generic;
using DuckGame;

namespace DuckGame.BrutalDG
{
	internal class UIMenuItemNumberOld : UIMenuItem
	{
		public UIMenuItemNumberOld(string text, UIMenuAction action = null, FieldBinding field = null, int step = 1, Color c = default(Color), FieldBinding upperBoundField = null, FieldBinding lowerBoundField = null, string append = "", FieldBinding filterField = null, List<string> valStrings = null, MatchSetting setting = null) : base(action, default(Color))
		{
			this._setting = setting;
			if (c == default(Color))
			{
				c = Colors.MenuOption;
			}
			this._valueStrings = valStrings;
			UIDivider splitter = new UIDivider(true, (this._valueStrings != null) ? 0f : 0.8f, 1f);
			UIText t = new UIText(text, c, UIAlign.Center, 0f, null);
			t.align = UIAlign.Left;
			splitter.leftSection.Add(t, true);
			if (field == null)
			{
				this._textItem = new UIChangingText(-1f, -1f, field, null);
				this._textItem.align = UIAlign.Right;
				splitter.rightSection.Add(this._textItem, true);
			}
			else if (this._valueStrings != null)
			{
				if (text == "" || text == null)
				{
					splitter.leftSection.align = UIAlign.Left;
					this._textItem = t;
					int newVal = (int)field.value;
					if (newVal >= 0 && newVal < this._valueStrings.Count)
					{
						this._textItem.text = this._valueStrings[newVal];
					}
				}
				else
				{
					this._textItem = new UIChangingText(-1f, -1f, field, null);
					int newVal2 = (int)field.value;
					if (newVal2 >= 0 && newVal2 < this._valueStrings.Count)
					{
						this._textItem.text = this._valueStrings[newVal2];
					}
					this._textItem.align = UIAlign.Right;
					splitter.rightSection.Add(this._textItem, true);
				}
			}
			else
			{
				UINumber number = new UINumber(-1f, -1f, field, append, filterField, this._setting);
				number.align = UIAlign.Right;
				splitter.rightSection.Add(number, true);
			}
			if (this._valueStrings != null)
			{
				string longest = "";
				foreach (string r in this._valueStrings)
				{
					if (r.Length > longest.Length)
					{
						longest = r;
					}
				}
				this._textItem.text = this._textItem.text;
			}
			base.rightSection.Add(splitter, true);
			this._arrow = new UIImage("contextArrowRight", UIAlign.Left);
			this._arrow.align = UIAlign.Right;
			this._arrow.visible = false;
			base.leftSection.Add(this._arrow, true);
			this._field = field;
			this._step = step;
			this._upperBoundField = upperBoundField;
			this._lowerBoundField = lowerBoundField;
			this._filterField = filterField;
			this.controlString = "@CANCEL@BACK @WASD@ADJUST";
		}

		private int GetStep(int current, bool up)
		{
			if (this._setting == null || this._setting.stepMap == null)
			{
				return this._step;
			}
			int step = 0;
			foreach (KeyValuePair<int, int> pair in this._setting.stepMap)
			{
				step = pair.Value;
				if (up && pair.Key > current)
				{
					break;
				}
				if (!up && pair.Key >= current)
				{
					break;
				}
			}
			return step;
		}

		public override void Activate(string trigger)
		{
			if (this._filterField != null)
			{
				if (!(bool)this._filterField.value && (trigger == "MENURIGHT" || trigger == "SELECT"))
				{
					SFX.Play("textLetter", 0.7f, 0f, 0f, false);
					this._filterField.value = true;
					this._field.value = (int)this._field.min;
					return;
				}
				if (!(bool)this._filterField.value && trigger == "MENULEFT")
				{
					SFX.Play("textLetter", 0.7f, 0f, 0f, false);
					this._filterField.value = true;
					this._field.value = (int)this._field.max;
					return;
				}
				if ((bool)this._filterField.value && trigger == "MENULEFT" && (float)((int)this._field.value) == this._field.min)
				{
					SFX.Play("textLetter", 0.7f, 0f, 0f, false);
					this._filterField.value = false;
					return;
				}
				if ((bool)this._filterField.value && (trigger == "MENURIGHT" || trigger == "SELECT") && (float)((int)this._field.value) == this._field.max)
				{
					SFX.Play("textLetter", 0.7f, 0f, 0f, false);
					this._filterField.value = false;
					return;
				}
				if (this._setting != null && trigger == "MENU2")
				{
					SFX.Play("textLetter", 0.7f, 0f, 0f, false);
					if (this._setting.filterMode == FilterMode.GreaterThan)
					{
						this._setting.filterMode = FilterMode.Equal;
						return;
					}
					if (this._setting.filterMode == FilterMode.Equal)
					{
						this._setting.filterMode = FilterMode.LessThan;
						return;
					}
					if (this._setting.filterMode == FilterMode.LessThan)
					{
						this._setting.filterMode = FilterMode.GreaterThan;
					}
					return;
				}
			}
			int prev = (int)this._field.value;
			if (trigger == "MENULEFT")
			{
				this._field.value = (int)this._field.value - this.GetStep((int)this._field.value, false);
			}
			else if (trigger == "MENURIGHT" || trigger == "SELECT")
			{
				this._field.value = (int)this._field.value + this.GetStep((int)this._field.value, true);
			}
			int newVal = (int)Maths.Clamp((float)((int)this._field.value), this._field.min, this._field.max);
			if (this._upperBoundField != null && newVal > (int)this._upperBoundField.value)
			{
				this._upperBoundField.value = newVal;
			}
			if (this._lowerBoundField != null && newVal < (int)this._lowerBoundField.value)
			{
				this._lowerBoundField.value = newVal;
			}
			if (prev != (int)this._field.value)
			{
				SFX.Play("textLetter", 0.7f, 0f, 0f, false);
			}
			int dif = newVal - prev;
			this._field.value = newVal;
			if (dif > 0)
			{
				int totalPercent = dif;
				using (List<FieldBinding>.Enumerator enumerator = this.percentageGroup.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FieldBinding p = enumerator.Current;
						while ((float)((int)p.value) > p.min && totalPercent > 0)
						{
							int newPVal = (int)p.value;
							newPVal -= (int)p.inc;
							p.value = newPVal;
							totalPercent -= (int)p.inc;
						}
					}
					goto IL_4E9;
				}
			}
			if (dif < 0)
			{
				int totalPercent2 = dif;
				foreach (FieldBinding p2 in this.percentageGroup)
				{
					while ((float)((int)p2.value) < p2.max && totalPercent2 < 0)
					{
						int newPVal2 = (int)p2.value;
						newPVal2 += (int)p2.inc;
						p2.value = newPVal2;
						totalPercent2 += (int)p2.inc;
					}
				}
			}
		IL_4E9:
			if (this._textItem != null && newVal >= 0 && newVal < this._valueStrings.Count)
			{
				this._textItem.text = this._valueStrings[newVal];
			}
		}

		protected FieldBinding _field;

		protected int _step;

		protected FieldBinding _upperBoundField;

		protected FieldBinding _lowerBoundField;

		protected FieldBinding _filterField;

		protected UIText _textItem;

		public List<FieldBinding> percentageGroup = new List<FieldBinding>();

		private List<string> _valueStrings;

		private MatchSetting _setting;
	}
}
