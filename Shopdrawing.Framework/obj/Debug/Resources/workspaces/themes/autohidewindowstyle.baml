   M S B A M L   `   `   `  ����P  LShopdrawing.Framework, Version=4.1.0.0, Culture=neutral, PublicKeyToken=nulls>clr-namespace:Microsoft.VisualStudio.PlatformUI.Shell.Controls0Microsoft.VisualStudio.PlatformUI.Shell.Controls  5      U/clr-namespace:Microsoft.VisualStudio.PlatformUI!Microsoft.VisualStudio.PlatformUI  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089E    ?Microsoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideWindow  �� 
TargetType �� 	ResourcesK   EMicrosoft.VisualStudio.PlatformUI.Shell.Controls.SubtractionConverter   Key   
Subtrahend �� Property �� Value �� 
TargetTypeQ   KMicrosoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideHwndContentControl  NameI   CMicrosoft.VisualStudio.PlatformUI.Shell.Controls.HwndContentControl   Content ��Name	 � RowDefinitions
 � ColumnDefinitionsG   AMicrosoft.VisualStudio.PlatformUI.Shell.Controls.WindowResizeGrip  Name   ResizeTarget �� RelativeSource �� TriggersM   GMicrosoft.VisualStudio.PlatformUI.Shell.Controls.AutoHideChannelControl P� Property P� Value �� 
TargetName �� Path �� Mode  SubtractionConverter �� 	Converter�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      Evs>clr-namespace:Microsoft.VisualStudio.PlatformUI.Shell.Controls  5      6ui/clr-namespace:Microsoft.VisualStudio.PlatformUI  5      ResourceDictionary_1��%�  '         7  7  7  7  �� 5      Style_1��"    6    5      ��  5      vs:SubtractionConverter_1��SubtractionConverter  $ 20Q�6_   5	      .�5
      �� Setter_1�� � �� 5      �� 5   
   5      �� 5      Setter_2�� � �� �#F0F0F06<   �� 5      Setter_3�� � �� 5      �� 5   
   ControlTemplate_1��"   64   .R�5        $ PART_HwndHost��6*   PART_HwndHost��. 5      �� $ OuterBorder��6   OuterBorder��#��z��6>   
���   �$��0q�5   )   ._�5      �� $ Border��6   
Border�����#A0A0A0$��0q�6L   ._�5      � 
Grid_1��	 5      �� 5      RowDefinition_1��$
��Auto �6<   �� 5      RowDefinition_2��6$   �� 5      RowDefinition_3��$
��Auto �6<   
 5      �� 5      ColumnDefinition_1��$
��Auto �6B   �� 5      ColumnDefinition_2��6'   �� 5       ColumnDefinition_3��$
��Auto �6B   .+�5"      �� ContentPresenter_1��$��1��6@   $��1��6M   $��Content��6]     5#      $ 
ResizeGrip��6(   
ResizeGrip�� 5$      �� 5%       6    �� *TemplatedParent+5'      5(      5)      5*      5+       5,      P� 5-      	Trigger_1�� �@ ChannelDock$
 Left=�6Y   . �5.      �� Setter_4��$ 
ResizeGrip��6(    � ��$ 2��6W   �� 5/      Setter_5��$ 
ResizeGrip��6(    � ��$ 3��6X   �� 50      Setter_6��$ 
ResizeGrip��6(    � ��$ 4��6b   �� 51      Setter_7��$ 
ResizeGrip��6(    �@ ResizeGripDirection$ Right=�52      �� 53      Setter_8��$ Border��6(    � ��$ 0,0,1,0q�6^   �� 54      Setter_9��$ OuterBorder��6(    � ��$ 0,0,1,0q�6c   �� 55      	Setter_10�� � �� 56      �� 57      $ AutoHideWidth�6   $ TwoWay=�61   59      �� 5:      	Setter_11�� 6)   �� *MinimumWidth+ � ��6H   �� 5;      	Setter_12�� � �� 5<      �� 5=      $! DockRootElement.ActualWidth�6   8    6?   �� *Self+5>      5@      5A      P� 5B      	Trigger_2�� �@ ChannelDock$ Right=�6Y   . �5C      �� 	Setter_13��$ 
ResizeGrip��6)    � ��$ 3��6Y   �� 5D      	Setter_14��$ 
ResizeGrip��6)    � ��$ 4��6c   �� 5E      	Setter_15��$ 
ResizeGrip��6)    �@ ResizeGripDirection$
 Left=�5F      �� 5G      	Setter_16��$ Border��6)    � ��$ 1,0,0,0q�6_   �� 5H      	Setter_17��$ OuterBorder��6)    � ��$ 1,0,0,0q�6d   �� 5I      	Setter_18�� � �� 5J      �� 5K      $ AutoHideWidth�6   $ TwoWay=�61   5M      �� 5N      	Setter_19�� 6)   �� *MinimumWidth+ � ��6H   �� 5O      	Setter_20�� � �� 5P      �� 5Q      $! DockRootElement.ActualWidth�6   8   6?   �� *Self+5R      5T      5U      P� 5V      	Trigger_3�� �@ ChannelDock$	 Top=�6Y   . �5W      �� 	Setter_21��$ 
ResizeGrip��6)    � ��$ 2��6U   �� 5X      	Setter_22��$ 
ResizeGrip��6)    � ��$ 3��6\   �� 5Y      	Setter_23��$ 
ResizeGrip��6)    � ��$ 4��6d   �� 5Z      	Setter_24��$ 
ResizeGrip��6)    �@ ResizeGripDirection$ Bottom=�5[      �� 5\      	Setter_25��$ Border��6)    � ��$ 0,0,0,1q�6_   �� 5]      	Setter_26��$ OuterBorder��6)    � ��$ 0,0,0,1q�6d   �� 5^      	Setter_27�� � �� 5_      �� 5`      $ AutoHideHeight�6   $ TwoWay=�62   5b      �� 5c      	Setter_28�� 6)   �� *MinimumHeight+ � ��6I   �� 5d      	Setter_29�� � �� 5e      �� 5f      $" DockRootElement.ActualHeight�6   8   6@   �� *Self+5g      5i      5j      P� 5k      	Trigger_4�� �@ ChannelDock$ Bottom=�6Y   . �5l      �� 	Setter_30��$ 
ResizeGrip��6)    � ��$ 3��6\   �� 5m      	Setter_31��$ 
ResizeGrip��6)    � ��$ 4��6d   �� 5n      	Setter_32��$ 
ResizeGrip��6)    �@ ResizeGripDirection$	 Top=�5o      �� 5p      	Setter_33��$ Border��6)    � ��$ 0,1,0,0q�6_   �� 5q      	Setter_34��$ OuterBorder��6)    � ��$ 0,1,0,0q�6d   �� 5r      	Setter_35�� � �� 5s      �� 5t      $ AutoHideHeight�6   $ TwoWay=�62   5v      �� 5w      	Setter_36�� 6)   �� *MinimumHeight+ � ��6I   �� 5x      	Setter_37�� � �� 5y      �� 5z      $" DockRootElement.ActualHeight�6   8   6@   �� *Self+5{      5}      5~      5�      5�      5�      5�      