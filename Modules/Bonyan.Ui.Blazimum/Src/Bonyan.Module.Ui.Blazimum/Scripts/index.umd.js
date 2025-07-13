// index.umd.js
import Collapsible from './components/collapsible.js';
import DOMUtils from './utils/domUtils';

const Blazimum = {
    // Use `createInstance` method to create instances in a standardized way
    Collapsible,
    DOMUtils
};

// Attach to `window` for global use
if (typeof window !== 'undefined') {
    window.Blazimum = Blazimum;
}

export default Blazimum;
