// /components/baseComponent.js
export default class BaseComponent {
    // Static factory method to create an instance
    static createInstance(...args) {
        return new this(...args); // `this` refers to the derived class
    }
}
