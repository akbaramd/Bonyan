class Collapsible {

    static initialize(id, isOpen) {
        const element = document.getElementById(id);
        if (element) {
            // Set initial open/closed state without animation
            if (isOpen) {
                element.style.maxHeight = `${element.scrollHeight}px`;
                element.style.opacity = '1';
            } else {
                element.style.maxHeight = '0px';
                element.style.opacity = '0';
            }

            // Disable transition for initial setup
            element.style.transition = 'none';

            // Trigger reflow for immediate style application
            void element.offsetHeight;

            // Enable transition for animations after the initial state
            setTimeout(() => {
                element.style.transition = 'max-height 0.4s ease, opacity 0.4s ease';
            }, 0);
        }
    }

    static toggle(id, isOpen) {
        const element = document.getElementById(id);
        if (element) {
            // Update maxHeight based on isOpen, handling nested content by toggling between fixed height and auto
            if (isOpen) {
                element.style.maxHeight = `${element.scrollHeight}px`; // Start animation to expand
                element.style.opacity = '1';

                // Use auto after transition completes to allow height to expand with nested content
                element.addEventListener('transitionend', function handleTransitionEnd() {
                    element.style.maxHeight = 'none';
                    element.removeEventListener('transitionend', handleTransitionEnd);
                });
            } else {
                // Collapse with animation
                element.style.maxHeight = `${element.scrollHeight}px`; // Start from current height
                void element.offsetHeight; // Trigger reflow to apply styles
                element.style.maxHeight = '0px'; // Animate to collapsed
                element.style.opacity = '0';
            }
        }
    }
}

export default Collapsible;
