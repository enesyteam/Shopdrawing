   M S B A M L   `   `   `  ����P  LShopdrawing.Framework, Version=4.1.0.0, Culture=neutral, PublicKeyToken=nulls>clr-namespace:Microsoft.VisualStudio.PlatformUI.Shell.Controls0Microsoft.VisualStudio.PlatformUI.Shell.Controls  5      a5clr-namespace:Microsoft.VisualStudio.PlatformUI.Shell'Microsoft.VisualStudio.PlatformUI.Shell  5      a5clr-namespace:Microsoft.Expression.Framework.Controls'Microsoft.Expression.Framework.Controls  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F    @Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideTabItem  �� 
TargetType �� 	Resources �� DataType2   ,Microsoft.VisualStudio.PlatformUI.Shell.View ��Name �� Triggers �� Value �� Binding �� ElementName �� Path	 �� 
TargetName
 �� Property �� Value �� AngleA   ;Microsoft.VisualStudio.PlatformUI.Shell.Controls.MouseHover9   3Microsoft.VisualStudio.PlatformUI.Shell.ViewManager   Instance �� Source �� ModeC   =Microsoft.VisualStudio.PlatformUI.Shell.Controls.ViewCommands$   ShowAndActivateAutoHiddenView $   PaletteTabDefaultBackgroundBrush $  PaletteTabDefaultForegroundBrush �� 
TargetType ��Name �� CornerRadiusK   EMicrosoft.Expression.Framework.Controls.PositioningTextBlockContainer J� SnapsToDevicePixels �� Triggers �� Triggers P� Property P� Value & "PaletteTabMouseOverBackgroundBrush�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Evs>clr-namespace:Microsoft.VisualStudio.PlatformUI.Shell.Controls  5      Avsshell5clr-namespace:Microsoft.VisualStudio.PlatformUI.Shell  5      ?fwctl5clr-namespace:Microsoft.Expression.Framework.Controls  5      ResourceDictionary_1��%	  '         �� 5      Style_1��"    6    5      ���� 5	      DataTemplate_1��"  6,   .R�5
   
   �� $ 	Presenter��6   	Presenter����6>   �� *Title+��5      �� *TabTitleTemplate+ 5   
   �� 5      DataTrigger_1��$ Vertical��6.    5      �� 5      $ 	Presenter��6   $- '(vs:AutoHideChannelControl.Orientation)�60   .a�5      �� Setter_1��$	 	Presenter��6&   
 �@�LayoutTransform 5      �� 5      RotateTransform_1��$ 90Q�6<   5      5      5   	   5      .�5      �� Setter_4��
 �@ MouseHoverDelay 5      �� 5   
   # Z 6   $$ Preferences.AutoHideHoverDelay�6C   $ OneTime=�5      5      �� 5       Setter_5��#
 �@ IsMouseHoverTrackingEnabled . 6S   �� 5!      Setter_7��
 �@  IsAutoHideWindowShown 6R   �� *
IsSelected+�� 5"      Setter_8��
 � ��# Z 5#      �� 5$      Setter_9��
 � �� 5%      �� 5&   
   5(      �� 5)      	Setter_10��
 � ��# � ��5*      �� 5+      	Setter_11��
 � ��# � ��5,      �� 5-      	Setter_12��
 � ��# � ��5.      �� 5/      	Setter_13��
 �@J�SnapsToDevicePixels . 6H   �� 50      	Setter_14��
 � ��# �   51      �� 52      	Setter_15��
 � ��# �  53      �� 54      	Setter_19��
 � ��$ -1,0,0,0q�6B   �� 55      	Setter_16��
 � �� 56      �� 57   
   ControlTemplate_1��"   64   .R�58      �� $ TabItem��6   TabItem��#��z��64   #��z��59      $ 3,3,0,0��6Q   $��0,0,1,0q�5:      ._�5;        )%fwctl:PositioningTextBlockContainer_1�� .  ._�5<      �� TextBlock_1��$��Stretch=�6.   $��Center=�5=      $��8,3,8,3q�6F   ��5>      �� *Title+5?      5@       5A      �� 5B      DataTrigger_2��$ Vertical��60    5C      �� 5D      $ TabItem��6   $- '(vs:AutoHideChannelControl.Orientation)�60   .a�5F      �� Setter_3��
 �@�LayoutTransform 5G      �� 5H      RotateTransform_2��$ 90Q�6>   5J      5K      5M      5O       5P      P� 5Q      	Trigger_1�� �@J�IsMouseOver . . �5R   
   �� 	Setter_17��
 � ��# �  5S      �� 5T   
   	Setter_18��
 � ��# �  5U      5V   	   5X      5Y      