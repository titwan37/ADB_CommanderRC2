/*
 * Created by SharpDevelop.
 * User: FalempiA
 * Date: 27.01.2016
 * Time: 15:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;


namespace WPF_S39_Commander.Control
{
	/// <summary>
	/// Description of MediaItem .
	/// </summary>
public class MediaItem : INotifyPropertyChanged
{
    private bool _isEditing;
    private bool _isSelected;
    private string _label;

    public MediaItem()
    {
        IsEditing = false;
        _isSelected = false;
    }

    public bool IsEditing
    {
        get { return _isEditing; }
        set
        {
            if (_isEditing == value) return;
            _isEditing = value;
            OnPropertyChanged("IsEditing");
        }
    }

    public string Label
    {
        get { return _label; }
        set
        {
            _label = value;
            OnPropertyChanged("Label");
        }
    }

    public DateTime Date { get; set; }
    public string IconPath { get; set; }

    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            OnPropertyChanged("IsSelected");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    //[NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
