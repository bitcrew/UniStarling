using System;

namespace starling.events{
	
	public class Event{
	
		/** Event type for a display object that is added to a parent. */
        public const string ADDED = "added";
        /** Event type for a display object that is added to the stage */
        public const string ADDED_TO_STAGE = "addedToStage";
        /** Event type for a display object that is entering a new frame. */
        public const string ENTER_FRAME = "enterFrame";
        /** Event type for a display object that is removed from its parent. */
        public const string REMOVED = "removed";
        /** Event type for a display object that is removed from the stage. */
        public const string REMOVED_FROM_STAGE = "removedFromStage";
        /** Event type for a triggered button. */
        public const string TRIGGERED = "triggered";
        /** Event type for a display object that is being flattened. */
        public const string FLATTEN = "flatten";
        /** Event type for a resized Flash Player. */
        public const string RESIZE = "resize";
        /** Event type that may be used whenever something finishes. */
        public const string COMPLETE = "complete";
        /** Event type for a (re)created stage3D rendering context. */
        public const string CONTEXT3D_CREATE = "context3DCreate";
        /** Event type that indicates that the root DisplayObject has been created. */
        public const string ROOT_CREATED = "rootCreated";
        /** Event type for an animated object that requests to be removed from the juggler. */
        public const string REMOVE_FROM_JUGGLER = "removeFromJuggler";
        
        /** An event type to be utilized in custom events. Not used by Starling right now. */
        public const string CHANGE = "change";
        /** An event type to be utilized in custom events. Not used by Starling right now. */
        public const string CANCEL = "cancel";
        /** An event type to be utilized in custom events. Not used by Starling right now. */
        public const string SCROLL = "scroll";
        /** An event type to be utilized in custom events. Not used by Starling right now. */
        public const string OPEN = "open";
        /** An event type to be utilized in custom events. Not used by Starling right now. */
        public const string CLOSE = "close";
        /** An event type to be utilized in custom events. Not used by Starling right now. */
        public const string SELECT = "select";
	}
}