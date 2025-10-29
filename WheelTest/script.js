// // Source: https://codepen.io/wheatup/pen/GbgyLY

// OPTIONAL: keep this if you still want to block context menus
document.body.addEventListener('contextmenu', e => e.preventDefault());

const wheel = document.querySelector('.wheel');
const arcs  = Array.from(wheel.querySelectorAll('.arc'));

let isOpen = false;

function openWheel(x, y) {
  wheel.style.setProperty('--x', `${x}px`);
  wheel.style.setProperty('--y', `${y}px`);
  wheel.setAttribute('data-chosen', 0);
  wheel.classList.add('on');
  isOpen = true;
}

function closeWheel() {
  wheel.classList.remove('on');
  wheel.setAttribute('data-chosen', 0);
  isOpen = false;
}

/* Click anywhere to open (unless you clicked a slice while open) */
document.addEventListener('click', (e) => {
  const arc = e.target.closest('.arc');

  // If the wheel is open and a slice was clicked → select it.
  if (isOpen && arc) {
    const index = arcs.indexOf(arc) + 1; // 1..N
    wheel.setAttribute('data-chosen', index);

    // TODO: trigger your action here
    // handleChoice(index);

    closeWheel();
    return;
  }

  // If the wheel is open and you clicked outside → close it.
  if (isOpen && !arc && !e.target.closest('.wheel')) {
    closeWheel();
    return;
  }

  // If the wheel is closed → open it at the click position.
  if (!isOpen) {
    openWheel(e.clientX, e.clientY);
  }
});

/* Esc to close */
document.addEventListener('keydown', (e) => {
  if (e.key === 'Escape' && isOpen) closeWheel();
});

/* Optional: preview highlight on hover while open */
arcs.forEach((arc, i) => {
  arc.addEventListener('mouseenter', () => {
    if (!isOpen) return;
    wheel.setAttribute('data-chosen', i + 1);
  });
  arc.addEventListener('mouseleave', () => {
    if (!isOpen) return;
    wheel.setAttribute('data-chosen', 0);
  });
});


// document.body.addEventListener('contextmenu', e => e.preventDefault() & e.stopPropagation());
// document.body.addEventListener('mousedown', onMouseDown);
// document.body.addEventListener('touchstart', e => onMouseDown(e.touches[0]));

// document.body.addEventListener('touchend', e => onMouseUp(e.touches[0]));
// document.body.addEventListener('mousemove', onMouseMove);
// document.body.addEventListener('touchmove', e => onMouseMove(e.touches[0]));

// let showing, anchorX, anchorY, min = 100;
// const wheel = document.querySelector('.wheel');

// function onMouseDown({ clientX: x, clientY: y }) {
//   showing = true;
//   anchorX = x;
//   anchorY = y;
//   wheel.style.setProperty('--x', `${x}px`);
//   wheel.style.setProperty('--y', `${y}px`);
//   wheel.classList.add('on');
// }

// function onMouseUp() {
//   showing = false;
//   wheel.setAttribute('data-chosen', 0);
//   wheel.classList.remove('on');
// }

// function onMouseMove({ clientX: x, clientY: y }) {
//   if (!showing) return;

//   const dx = x - anchorX;
//   const dy = y - anchorY;
//   const mag = Math.hypot(dx, dy);
//   let index = 0;

//   if (mag >= min) {
//     const sectors = 5;
//     const step = (Math.PI * 2) / sectors;      // 72°
//     const offset = Math.PI / 2; //+ step / 2;     // 90° + 36° = 126° = 0.7π

//     let ang = Math.atan2(dy, dx) + offset;

//     // normalize to [0, 2π)
//     while (ang < 0) ang += Math.PI * 2;
//     while (ang >= Math.PI * 2) ang -= Math.PI * 2;

//     index = Math.floor((ang / (Math.PI * 2)) * sectors) + 1; // 1..5
//   }

//   wheel.setAttribute('data-chosen', index);
// }
