   M S B A M L   `   `   `  ����?  ;Exam, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:Exam.MVVM.ModelViewExam.MVVM.ModelView  5      &     Exam.MVVM.ModelView.PassTestView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      3local!clr-namespace:Exam.MVVM.ModelView     5      .��5
      �   � RowDefinitions  5   
   �� 5      $��50 �6   �� 5      .+�5   
   �� -   5        $��
TestsTitle��5      $	��250��5      $
��Left=�5      
��� � �$��10q�5       � ToolTip$% Enter test which u want to pass��5       5      �� 5   
   -   5         ��Name$ TestTitleTb��5      $��Center=�5       $��Center=�5!      $	��411��5"      ���#353440$��18(�5$       �� TextAlignment$ Center=�5%      
�������   EmptyTextBox#��[  5'      $��260,5,129,5q�6>    �� 
IsReadOnly . .��5)      5*      �� 5,   
   -   5         ��Name$ StartTestBtn��5,      $��Bottom=�5-      $��Right=�5.      $��
Start test��5/        ButtonTheme#��[ 50      $��46��51      $	��120��52      $��0,0,0,2q�68   � 54   
   $��1��6    � ColumnDefinitions 55      �� 56      �� 57      .+�5;      �� -   5        $��StackPanelWithCB��5;      .+�5<      �� -   5        $��FirstVariantInChB��5B   #   $�� ��5<      $��40��5=   #   $��10, 30, 10, 15q�5>   "   $��Center=�5?   "   $��Stretch=�5@   #     CheckBoxTheme#��[ 5A   #   
�������5E      �� 5G      -   5        $��SecondVariantInChB��5M   #   $�� ��5G      $��40��5H   #   $��10, 30, 10, 15q�5I   "   $��Center=�5J   "   $��Stretch=�5K   #   #��[ 5L   #   
�������5O      �� 5Q      -   5        $��ThirdVariantInChB��5W   #   $�� ��5Q      $��40��5R   #   $��10, 30, 10, 15q�5S   "   $��Center=�5T   "   $��Stretch=�5U   #   #��[ 5V   #   
�������5Y      �� 5[      -   5        $��FourthVariantInChB��5a   #   $�� ��5[      $��40��5\   #   $��10, 30, 10, 15q�5]   "   $��Center=�5^   "   $��Stretch=�5_   #   #��[ 5`   #   
�������5c      5d      � 5f      $��1��6     5g      �� 5h      �� 5i      $��50 �6$   .+�5m      �� -	   5        $ 
QuestionTb��5q      $��40��5m      $	��Top=�5n      $��10q�5o      #��[  5p      $y�Hidden=�5r      {�.  5s   .   �� 5u      -
   5        $ QuestiobVariantsInfoTb��5y      $��40��5u      $	��Top=�5v      $��10,61,10,10q�5w      #��[  5x      $y�Hidden=�5z      �� 5~      -   5         ��Name$ QuestionImage��5~      $��10,106,10,10q�5      �� 5�      $��1��6    �� Orientation$ 
Horizontal=�5�      .+�5�      �� -   5        $ FinishTestBtn��5�      $��40, 5, 5, 5q�5�      $��40��5�      $	��140��5�        NewQuestionTheme#��[ 5�      $��Finish test��5�      63   �� 5�      -   5        $ NextQuestionBtn��5�      $��20, 5, 5, 5q�5�      $��40��5�      $	��140��5�      #��[ 5�      $��Next question��5�      65   5�      5�      5�      5�      5�      