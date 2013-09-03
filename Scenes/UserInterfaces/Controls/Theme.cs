using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Scenes.UserInterfaces.Controls
{
	/// <summary>
	/// Holds a set of materials and colors for Scenes UI controls, as well as the font to be used.
	/// </summary>
	public class Theme
	{
		public static Theme Default
		{
			get { return defaultTheme ?? (defaultTheme = new Theme()); }
		}

		private static Theme defaultTheme;

		public Theme()
		{
			DefaultButtonAppearance();
			DefaultDropdownListAppearance();
			Font = Rendering.Fonts.FontXml.Default;
			Label = new Appearance("DefaultLabel");
			DefaultRadioButtonAppearance();
			DefaultScrollbarAppearance();
			DefaultSelectBoxAppearance();
			DefaultSliderAppearance();
			DefaultTextBoxAppearance();
		}
		public FontXml Font { get; set; }
		public Appearance Label { get; set; }

		public struct Appearance
		{
			public Appearance(string materialName)
				: this(new Material(Shader.Position2DColorUv, materialName)) { }

			public Appearance(Material material)
				: this()
			{
				Material = material;
				Color = Material.DefaultColor;
			}

			public Material Material { get; set; }
			public Color Color { get; set; }

			public Appearance(string materialName, Color color)
				: this()
			{
				Material = new Material(Shader.Position2DColorUv, materialName);
				Color = color;
			}
		}

		private void DefaultButtonAppearance()
		{
			Button = new Appearance("DefaultButtonBackground", Color.LightGray);
			ButtonDisabled = new Appearance("DefaultButtonBackground", Color.Grey);
			ButtonMouseover = new Appearance("DefaultButtonBackground");
			ButtonPressed = new Appearance("DefaultButtonBackground", Color.LightBlue);
		}

		public Appearance Button { get; set; }
		public Appearance ButtonDisabled { get; set; }
		public Appearance ButtonMouseover { get; set; }
		public Appearance ButtonPressed { get; set; }

		private void DefaultDropdownListAppearance()
		{
			DropdownListBox = new Appearance("DefaultLabel");
			DropdownListBoxDisabled = new Appearance("DefaultLabel", Color.Grey);
		}

		public Appearance DropdownListBox { get; set; }
		public Appearance DropdownListBoxDisabled { get; set; }

		private void DefaultRadioButtonAppearance()
		{
			RadioButtonBackground = new Appearance("DefaultLabel");
			RadioButtonBackgroundDisabled = new Appearance("DefaultLabel", Color.Grey);
			RadioButtonDisabled = new Appearance("DefaultRadiobuttonOff", Color.Grey);
			RadioButtonNotSelected = new Appearance("DefaultRadiobuttonOff");
			RadioButtonNotSelectedMouseover = new Appearance("DefaultRadioButtonOffHover");
			RadioButtonSelected = new Appearance("DefaultRadiobuttonOn");
			RadioButtonSelectedMouseover = new Appearance("DefaultRadioButtonOnHover");
		}

		public Appearance RadioButtonBackground { get; set; }
		public Appearance RadioButtonBackgroundDisabled { get; set; }
		public Appearance RadioButtonDisabled { get; set; }
		public Appearance RadioButtonNotSelected { get; set; }
		public Appearance RadioButtonNotSelectedMouseover { get; set; }
		public Appearance RadioButtonSelected { get; set; }
		public Appearance RadioButtonSelectedMouseover { get; set; }

		private void DefaultScrollbarAppearance()
		{
			Scrollbar = new Appearance("DefaultButtonBackground");
			ScrollbarDisabled = new Appearance("DefaultButtonBackground", Color.Grey);
			ScrollbarPointer = new Appearance("DefaultButtonBackground", Color.LightGray);
			ScrollbarPointerDisabled = new Appearance("DefaultButtonBackground", Color.Grey);
			ScrollbarPointerMouseover = new Appearance("DefaultButtonBackground");
		}

		public Appearance Scrollbar { get; set; }
		public Appearance ScrollbarDisabled { get; set; }
		public Appearance ScrollbarPointer { get; set; }
		public Appearance ScrollbarPointerDisabled { get; set; }
		public Appearance ScrollbarPointerMouseover { get; set; }

		private void DefaultSelectBoxAppearance()
		{
			SelectBox = new Appearance("DefaultLabel");
			SelectBoxDisabled = new Appearance("DefaultLabel", Color.Grey);
		}

		public Appearance SelectBox { get; set; }
		public Appearance SelectBoxDisabled { get; set; }

		private void DefaultSliderAppearance()
		{
			Slider = new Appearance("DefaultButtonBackground");
			SliderDisabled = new Appearance("DefaultButtonBackground", Color.Grey);
			SliderPointer = new Appearance("DefaultSlider");
			SliderPointerDisabled = new Appearance("DefaultSlider", Color.Grey);
			SliderPointerMouseover = new Appearance("DefaultSliderHover");
		}

		public Appearance Slider { get; set; }
		public Appearance SliderDisabled { get; set; }
		public Appearance SliderPointer { get; set; }
		public Appearance SliderPointerDisabled { get; set; }
		public Appearance SliderPointerMouseover { get; set; }

		private void DefaultTextBoxAppearance()
		{
			TextBox = new Appearance("DefaultLabel", Color.LightGray);
			TextBoxFocussed = new Appearance("DefaultLabel");
			TextBoxDisabled = new Appearance("DefaultLabel", Color.Grey);
		}
		
		public Appearance TextBox { get; set; }
		public Appearance TextBoxDisabled { get; set; }
		public Appearance TextBoxFocussed { get; set; }
	}
}