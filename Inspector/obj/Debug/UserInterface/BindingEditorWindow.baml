   M S B A M L   `   `   `  ����  	InspectorC    =ChristianMoser.WpfInspector.UserInterface.BindingEditorWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � ShowInTaskbar  .     WindowBackgroundBrush#���   5       � Title$ Binding Editor��5      $	��500��6   $	��500��6(    � Icon$/ )/Inspector;component/Images/Inspector.png��65    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5	      $G A/Inspector;component/UserInterface/VisualResources/Inspector.xaml?�6%   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   � $��8q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#    5      �� 5      $
��Auto �6    �� 5      $��* �6    �� 5      $
��Auto �6    .+�5      �� $��8q�6   $��2��6$    �� Orientation$ 
Horizontal=�68   $��1��6Q   $	��Top=�6a   $
��Left=�6y   .+�5      �� $��0,0,8,0q�6   $��Source:��6-   $��Center=�6<   �� 5      $	 �� IsSynchronizedWithCurrentItem$
	 TrueY�6;   ��6   �� *	Sources+6;   5       �� 5!      $��1��6   $��2��6"   $��1q�66   
 �� CornerRadius$
 3��6J     DefaultBorderBrush#���  6[     DefaultBackgroundBrush#���  6�   ._�5"      R� -   5         R�Name$
 tree��5"        BindingEditorTreeViewStyle#���  6)   ��6^   �� *
SourceList+5#      �� 5$      $��2��6   $��2��6&   $��8q�6:   $ 
Horizontal=�6E   $
��Left=�6^   .+�5%      �� $��0,0,8,0q�6   $��Update Trigger:��6-   $��Center=�6D   �� 5&      $��0,0,8,0q�6   $
	 TrueY�6Y   $��Center=�6~   ��6,   �� *UpdateSourceTriggers+6~   �� 5'      $��0,0,8,0q�6   $��Mode:��6-   $��Center=�6:   �� 5(      $��Center=�69   $
	 TrueY�6T   ��6   �� *Modes+6T   5)      5*      �� 5+   
   $��1��6   $��8,0,8,8q�6;    �� 
IsReadOnly . ��6   �� *
Expression+6L   �� 5,   
   $��2��6   $ 
Horizontal=�6"   $��8,0,8,8q�6;   $��Right=�6L   .+�5-      �� -   5        .��5.      �� $��Ok��6   5/      �� 50      $��8,0,0,0q�6    �� IsCancel . .��51      �� $��Cancel��6   52      53      54      55      