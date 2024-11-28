Pool Pattern: Implemented for storing and reusing gems and bombs. Reduces CPU usage by avoiding unnecessary destruction and instantiation of identical objects.

Singleton: Custom singleton pattern applied to simplify interaction between object instances and classes. When used carefully, it streamlines class interrelations.

Observer: Replaced Update calls and most direct class interactions with UnityEvents. This reduces per-frame overhead and aligns with the Open-Closed Principle.

Centralized Data Handling: Eliminated duplicate data storage in objects (gems/bombs), leaving only input reactions. This optimizes CPU resource usage by consolidating logic.

Class Separation: Logic split into distinct classes for better code readability and reduced confusion.

Interface Communication: Primary class interactions occur strictly through interfaces, minimizing unnecessary access to public data.

Settings Management: Moved settings to ScriptableObject, enabling universal access and potential updates via Addressables.
