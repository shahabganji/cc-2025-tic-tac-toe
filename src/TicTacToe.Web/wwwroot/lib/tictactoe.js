function playSound() {
    clickSound.currentTime = 0;
    clickSound.play();
}

function toggleTheme() {
    document.body.classList.toggle("dark");
    document.body.classList.toggle("light");
}

function showConfetti(isWinner) {
    confetti({
        particleCount: 100,
        spread: 70,
        origin: {y: 0.6},
        colors: isWinner ? undefined : ['#ff0000']
    });
}

function showBootstrapAlert(message, type = 'danger', timeout = 5000) {
    const alertContainer = document.getElementById('alert-container');

    // Create the alert div
    const alert = document.createElement('div');
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.setAttribute('role', 'alert');
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    alertContainer.appendChild(alert);

    // Auto-dismiss after timeout
    setTimeout(() => {
        alert.classList.remove('show');
        alert.classList.add('hide');
        setTimeout(() => alert.remove(), 300); // Wait for fade out transition
    }, timeout);
}
