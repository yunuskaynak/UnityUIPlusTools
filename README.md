# UnityUIPlusTools | Improve performance and visual of your User Interface

A Unity UI extension for animated, robust, and easily configured sequential or parallel layout transitions.  
Designed for use with Unity's built-in HorizontalLayoutGroup or VerticalLayoutGroup — visually author your UI, then animate like a pro!

---

## 🎬 Preview

| Horizontal Layout Animation | Vertical Layout Animation |
|----------------------------|--------------------------|
| ![Horizontal](https://s12.gifyu.com/images/bs8RE.gif) | ![Vertical](https://s12.gifyu.com/images/bs8Rh.gif) |


## 🚀 Installation

1. **Copy** `CustomHorizontalLayout.cs` and/or `CustomVerticalLayout.cs` into your project (e.g. `Assets/Scripts/UI`).
2. **Dependencies:**
   - [DOTween (Free or Pro)](http://dotween.demigiant.com/)
   - [UniTask](https://github.com/Cysharp/UniTask) (install via UPM or as a package)

---

## 🛠️ Setup & Usage

1. **Design with Unity's built-in Layout Group:**
   - Add a `HorizontalLayoutGroup` or `VerticalLayoutGroup` to your container GameObject.
   - Configure spacing, padding, child alignment, etc. as you wish.

2. **Add the Custom Layout Component:**
   - Add `CustomHorizontalLayout` or `CustomVerticalLayout` to the same GameObject.
   - Keep the standard layout group for design-time visual setup.

3. **On Play:**
   - The script waits a frame, then disables the Unity layout group (locking in the slot positions).
   - All further animations are handled by the custom script, keeping children in order.

4. **Trigger Animations in Code:**

```csharp
// Get reference (via inspector, GetComponent, etc.)
CustomHorizontalLayout customLayout = GetComponent<CustomHorizontalLayout>();

// To animate activations (add, show, move in)
customLayout.ActivateChild(new RectTransform[] { child1, child2 }, allAtOnce: true);

// To animate deactivations (remove, hide, move out)
customLayout.DeactivateChild(new RectTransform[] { child3 }, allAtOnce: false);
