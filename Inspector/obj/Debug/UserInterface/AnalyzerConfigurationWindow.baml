   M S B A M L   `   `   `  ����  	InspectorK    EChristianMoser.WpfInspector.UserInterface.AnalyzerConfigurationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5         WindowBackgroundBrush#���   5        � WindowStartupLocation$  CenterOwner=�5       � Title$ Active Rules��5      $	��350��6   $	��400��6'    � Icon$/ )/Inspector;component/Images/Inspector.png��63    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $G A/Inspector;component/UserInterface/VisualResources/Inspector.xaml?�6%   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   ��   DefaultBackgroundBrush#���  6   ._�5      �� -   5         ��Name$ analyzerBox��5   !   $��8q�6   
������ $��0q�6O   $ �� IsSynchronizedWithCurrentItem$
 TrueY�6�   ��6c   �� *	Analyzers+ 5      	 �� Color��%�   (��       5   &   * SystemColors.HighlightBrushKey+)(�� %     5   &   *SystemColors.ControlBrushKey+)�� 5      $	 Transparent��6X   �� 5      $	 Transparent��6V   5      
 �� 
GroupStyle
 5      (  "System.Windows.Controls.GroupStyle  5         HeaderTemplate 5      �� 5      .R�5   "   ��   DefaultForegroundBrush#���  6,   $��0,4,0,0q�6b   $��22��6s   $��Center=�6   $
��Bold"�6�   ��6�   �� *Name+6�   5      5       ��5"      �� 5#      .R�5$      �� $��Center=�6%   $��24��6@    �� Orientation$ 
Horizontal=�6L   .+�5%      �� $��Center=�6'    d� 	IsChecked 6B   �� *
IsActive+ �� Mode$ TwoWay=��� 5&      $	��8,0q�6(   $��Center=�65   ��6P   �� *Name+5'      5(      5*      5+      �� 5,   
   $��8,8,8,8q�6   $��2��6&   $��Right=�63   .+�5-      �� -   5         �� 	IsDefault . .��5.      �� $��Close��6   5/      50      51      52      