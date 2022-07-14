// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ExtCore.Infrastructure;

namespace ExtCore.Events
{
  /// <summary>
  /// Overrides the <see cref="ExtensionBase">ExtensionBase</see> class and provides the ExtCore.Events extension information.
  /// </summary>
  public class Extension : ExtensionBase
  {
    /// <summary>
    /// Gets the name of the extension.
    /// </summary>
    public override string Name => "ExtCore.Events";
  }
}