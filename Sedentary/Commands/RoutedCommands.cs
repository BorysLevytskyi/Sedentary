using System.Windows.Input;

namespace Sedentary.Commands
{
    public class RoutedCommands
    {
        public static RoutedCommand ReplaceToSittingState = new RoutedCommand("ReplaceToSittingStates", typeof(RoutedCommands));
        public static RoutedCommand ReplaceToStandingState = new RoutedCommand("ReplaceToStandingState", typeof(RoutedCommands));
    }
}