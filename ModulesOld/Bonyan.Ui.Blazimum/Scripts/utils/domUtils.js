class DOMUtils {

    static setStyle(element, styleObject) {
        for (const [key, value] of Object.entries(styleObject)) {
            element.style[key] = value;
        }
    }

    static getScrollHeight(id) {
        return document.getElementById(id).scrollHeight;
    }
}

export default DOMUtils;
