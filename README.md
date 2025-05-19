
# CustomHorizontalLayout & CustomVerticalLayout

A Unity UI extension for animated, robust, and easily configured sequential or parallel layout transitions.  
Designed for use with Unity's built-in HorizontalLayoutGroup or VerticalLayoutGroup — visually author your UI, then animate like a pro!

## 🎬 Preview

<h3 align="center">Horizontal Layout Animation</h3>
<p align="center">
  <img src="https://s14.gifyu.com/images/bs8RE.gif" alt="Horizontal Layout Animation" border="0">
</p>

<h3 align="center">Vertical Layout Animation</h3>
<p align="center">
  <img src="https://s14.gifyu.com/images/bs8Rh.gif" alt="Vertical Layout Animation" border="0"/>
</p>

## 🚀 Installation

<ol>
<li><b>Copy</b> <code>CustomHorizontalLayout.cs</code> and/or <code>CustomVerticalLayout.cs</code> into your project (e.g. <code>Assets/Scripts/UI</code>).</li>
<li><b>Dependencies:</b>
  <ul>
    <li><a href="http://dotween.demigiant.com/">DOTween (Free or Pro)</a></li>
    <li><a href="https://github.com/Cysharp/UniTask">UniTask</a> (install via UPM or as a package)</li>
  </ul>
</li>
</ol>

## 🛠️ Setup & Usage

<ol>
<li>
  <b>Design with Unity's built-in Layout Group:</b>
  <ul>
    <li>Add a <code>HorizontalLayoutGroup</code> or <code>VerticalLayoutGroup</code> to your container GameObject.</li>
    <li>Configure spacing, padding, child alignment, etc. as you wish.</li>
  </ul>
</li>

<li>
  <b>Add the Custom Layout Component:</b>
  <ul>
    <li>Add <code>CustomHorizontalLayout</code> or <code>CustomVerticalLayout</code> to the same GameObject.</li>
    <li>Keep the standard layout group for design-time visual setup.</li>
  </ul>
</li>

<li>
  <b>On Play:</b>
  <ul>
    <li>The script waits a frame, then disables the Unity layout group (locking in the slot positions).</li>
    <li>All further animations are handled by the custom script, keeping children in order.</li>
  </ul>
</li>

<li>
  <b>Trigger Animations in Code:</b>
  <pre><code>
// Get reference (via inspector, GetComponent, etc.)
CustomHorizontalLayout customLayout = GetComponent&lt;CustomHorizontalLayout&gt;();

// To animate activations (add, show, move in)
customLayout.ActivateChild(new RectTransform[] { child1, child2 }, allAtOnce: true);

// To animate deactivations (remove, hide, move out)
customLayout.DeactivateChild(new RectTransform[] { child3 }, allAtOnce: false);
  </code></pre>
  <i>allAtOnce: true animates all together; false animates one by one in reverse order (last to first for deactivation).</i>
</li>
</ol>

## ⚙️ Inspector Parameters

<ul>
  <li><b>Move/Scale Animation Durations & Delays</b> — Fine-tune how fast each transition plays.</li>
  <li><b>Global Ease</b> — Set the DOTween easing curve for all animations.</li>
  <li><b>Use Scale Animation</b> — Toggle pop/squash/stretch globally (default ON).</li>
  <li><b>Spacing/Padding/Alignment</b> — Still configured via the Unity LayoutGroup for design.</li>
</ul>

---

## 💡 Best Practices

<ul>
  <li><b>Use the LayoutGroup in the Editor</b> for visual design (spacing, alignment, etc.).</li>
  <li>Do <b>not</b> destroy or reorder children at runtime without calling <code>InitChilds()</code> on the custom script.</li>
  <li>The script <b>locks out new animation requests</b> while animating (no glitches from spamming).</li>
  <li>Ideal for animated inventories, dynamic menus, carousels, and reflowing UI bars.</li>
</ul>

## 📷 Example Workflow

<ol>
  <li>Add <code>HorizontalLayoutGroup</code> (or <code>VerticalLayoutGroup</code>) to your GameObject.</li>
  <li>Adjust its settings for your visual needs.</li>
  <li>Add <code>CustomHorizontalLayout</code> (or <code>CustomVerticalLayout</code>) to the same GameObject.</li>
  <li>On play, Unity arranges your elements, then disables its layout group.  
     The custom layout now animates all changes and transitions.</li>
  <li>Use <code>ActivateChild()</code> and <code>DeactivateChild()</code> in code to animate elements as needed.</li>
</ol>

## 🧩 Additional Notes

<ul>
  <li>The Unity layout group is <b>automatically disabled at runtime</b> after initial layout.</li>
  <li>The custom script captures the layout group’s positions for smooth animation.</li>
  <li>You can always change animation durations, easing, and scale effects via the inspector.</li>
</ul>

## 🕊️ License

This work and all included scripts are dedicated to the public domain under the <a href="https://creativecommons.org/publicdomain/zero/1.0/">CC0 1.0 Universal (CC0 1.0) Public Domain Dedication</a>.

You can copy, modify, distribute and perform the work, even for commercial purposes, all without asking permission.

