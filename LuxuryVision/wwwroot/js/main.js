
// Mobile Menu Toggle
const menuIcon = document.querySelector('.menu-icon');
const navLinks = document.querySelector('.nav-links');

if (menuIcon && navLinks) {
    menuIcon.addEventListener('click', () => {
        navLinks.classList.toggle('active');
    });
}

// Scroll Animation (Intersection Observer)
const scrollElements = document.querySelectorAll('.animate-on-scroll');
const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('is-visible');
        }
    });
}, {
    threshold: 0.1
});

scrollElements.forEach(el => {
    observer.observe(el);
});
document.addEventListener('DOMContentLoaded', () => {
    const quantityControls = document.querySelectorAll('.quantity-control');

    quantityControls.forEach(control => {
        const input = control.querySelector('.quantity-input');
        const decrementBtn = control.querySelector('[data-action="decrement"]');
        const incrementBtn = control.querySelector('[data-action="increment"]');
        const min = parseInt(input.min, 10);

        decrementBtn.addEventListener('click', () => {
            let currentValue = parseInt(input.value, 10);
            if (currentValue > min) {
                input.value = currentValue - 1;
            }
        });

        incrementBtn.addEventListener('click', () => {
            let currentValue = parseInt(input.value, 10);
            input.value = currentValue + 1;
        });
    });
});