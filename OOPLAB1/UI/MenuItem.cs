namespace OOPLAB1.UI;

// One line of a menu: its text and what happens when it is chosen.
public sealed record MenuItem(string Title, Action Action);
