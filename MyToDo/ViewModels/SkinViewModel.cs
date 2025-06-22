using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MaterialDesignColors.ColorManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MyToDo.ViewModels
{
    public class SkinViewModel : BindableBase
    {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        public DelegateCommand<Object> ChangeHueCommand { get; private set; }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        private bool _isDarkTheme;

        public bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                if (SetProperty(ref _isDarkTheme, value))
                {
                    ModifyTheme(theme => theme.SetBaseTheme(value ? BaseTheme.Dark : BaseTheme.Light));
                }
            }
        }

        public SkinViewModel()
        {
            IsDarkTheme = _paletteHelper.GetTheme().Equals(BaseTheme.Dark);
            ChangeHueCommand = new DelegateCommand<object>(ChangeHue);
        }

        private void ChangeHue(object obj)
        {
            var hue = (Color)obj;

            Theme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(hue.Lighten());
            theme.PrimaryMid = new ColorPair(hue);
            theme.PrimaryDark = new ColorPair(hue.Darken());

            _paletteHelper.SetTheme(theme);
        }

        private static void ModifyTheme(Action<Theme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
