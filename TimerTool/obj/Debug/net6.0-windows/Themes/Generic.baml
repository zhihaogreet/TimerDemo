   M S B A M L   `   `   `  ����D  @TimerTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:TimerTool	TimerTool  5      W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089S OHandyControl, Version=3.4.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b   CustomWindowDefault  �� 
TargetType    TimerTool.CustomWindow �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� RelativeSource �� Mode �� AncestorType	    dWidth
    dHeight � RowDefinitions ��Name �� Opacity �� Color � Title  CloseGeometry'  !HandyControl.Controls.IconElement   Geometry   Height   Width#  System.Windows.SystemCommands   CloseWindowCommand  ButtonBaseStyle ��Name  
FullScreen   MaximizeWindowCommand �� ElementName  
ReduceForm   MinimizeWindowCommand ��Name ��Name �� Triggers ]� 
Conditions �� Property �� Value �� 
TargetName P� Property  P� Value! �� Triggers" �� BasedOn�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      0hc'https://handyorg.github.io/handycontrol  5      !localclr-namespace:TimerTool  5      %  &	        '   �    7   �� 5      "    6(   .�5	   
   ��  � ��
 �����6'   �� 5
   
    � ��$ 2q�6,   �� 5   
    � ��
 ���� 6(   �� 5   
    �@�
ResizeMode$ NoResize=�6'   �� 5   
    �@�ShowInTaskbar$ False��6*   �� 5   
    �@J�SnapsToDevicePixels$
 True��60   �� 5   
    �@�WindowStyle$
 None=�6(   �� 5   
    �@�SizeToContent$ Manual=�6*   �� 5   
    � �� 5      �� 5      "   6"   .R�5      �� $ grids��5      #��z��5      ���#E4EDF3#��z��5       5      �� *WinCornerRadius+ �� $ FindAncestor=�"   ._�5      (� $��mviewbox��6"   .��5      � #��z	 6#   #��z
 6D    5   "   �� 5   &   $
 row1��64   $
��Auto �6B   �� 5   &   $
 row2��64   $
��auto �6B   �� 5    &   $��* �64   .+�5"   "   ]� $��toptitle��5#   %   $	��128��5$   %   $
��40,0q�5%   %   $��Center=�5&   %   $��False��5'   %   ��5(   &   �� 5)   *   $ 0.001Q�6:   $ Black��6J   .+�5+   &   �� $��Title��5,   )   $��Center=�5-   )   $��48(�5.   )   ���#636870#��z 50   )   �� 51   &   $��80��52   )   $��70��53   )   # �  54   )   $ 40Q�55   )   $ 40Q�56   )   
������ $��0q�58   )   #��Z 59   )   $��Right=�5:   )   
���   �#���  5<   )   �� 5=   &   $ 	WindowMax��5>   )   $��80��5?   )   $��70��5@   )   # �  5A   )   $ 40Q�5B   )   $ 40Q�5C   )   
������ $��0q�5E   )   #��Z 5F   )   $��Right=�5H   )   
���   �#���  5J   )   $y�	Collapsed=�5K   )   ��5G   )   �� $ contents��5K   )   �� 5L   &   $ 	WindowMin��5M   )   $��80��5N   )   $��70��5O   )   # �  5P   )   $ 40Q�5Q   )   $ 40Q�5R   )   
������ $��0q�5T   )   #��Z 5U   )   $��Right=�5W   )   
���   �#���  5Y   )   $y�	Collapsed=�5Z   )   ��5V   )   �� $ contents��5Z   )   5[   #   �� 5\   "   $ topline��5]   %   $��1��5^   %   $��2��5_   %   ���#c5c7cb5`   %   � 5a   "   $��2��5b   %   $��Stretch=�5c   %   $��Stretch=�5d   %   .+�5e   &   �� $ contents��67   5f   #   5g      5h      5i       5j      ]� 5k       5l      �� 5m   "    �@  isFollowBigger$
 True��6F   �� 5n   "    �@�WindowState$ 	Maximized=�6C   /�5p      �� 5q   "   $ toptitle��6)    � y�$ Hidden=�6U   �� 5r   "   $ topline��6)    � y�$ Hidden=�6T   5t      ]� 5u       5v      �� 5w   "    �@  isFollowBigger$ False��6F   �� 5x   "    �@�WindowState$ 	Maximized=�6C   /�5z      �� 5{   "    �@  dWidth$
 3840Q�6;   �� 5|   "    �@  dHeight$
 2160Q�6<   �� 5}   "   $ toptitle��6)    � y�$ 	Collapsed=�6U   �� 5~   "   $ topline��6)    � y�$ 	Collapsed=�6T   �� 5   "   $ grids��6)    � ��$ 0q�6W   5�      ]� 5�       5�      �� 5�   "    �@  WinMinStauts$ MinHide=�6D   �� 5�   "    �@  IsMinStauts$
 True��6C   /�5�      �� 5�   "   $ toptitle��6)    � y�$ 	Collapsed=�6U   �� 5�   "   $ topline��6)    � y�$ 	Collapsed=�6T   �� 5�   "   $ grids��6)    � ��$ 0q�6W   5�      P� 5�       �@  IsShowTitleBar$  False��6<   . �5�      �� $ toptitle��6%    � y�$ 	Collapsed=�6Q   �� 5�      $ topline��6%    � y�$ 	Collapsed=�6P   5�      P� 5�       �@  IsShowTitleLine$  False��6=   . �5�      �� $ topline��6%    � y�$ 	Collapsed=�6P   5�      P� 5�       �@  IsShowMaxButton$
  True��6=   . �5�      �� $ 	WindowMax��6%    � y�$ Visible=�6R   5�      P� 5�       �@  IsShowMinButton$
  True��6=   . �5�      �� $ 	WindowMin��6%    � y�$ Visible=�6R   5�      5�      5�      ! 5�   
   5�      �� 5�      8"   "    6;   5�      