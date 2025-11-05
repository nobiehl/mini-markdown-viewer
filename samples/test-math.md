# Mathematical Formulas Test

This document tests the KaTeX mathematical formula rendering capabilities of the Markdown Viewer.

## Inline Math

Simple inline formulas using `$...$` syntax:

The Pythagorean theorem is $a^2 + b^2 = c^2$, which is one of the most famous equations in mathematics.

Einstein's mass-energy equivalence is given by $E = mc^2$, where $E$ is energy, $m$ is mass, and $c$ is the speed of light.

The quadratic formula is $x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}$.

## Block Math

Display formulas using `$$...$$` syntax:

$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

The Schrödinger equation:

$$
i\hbar\frac{\partial}{\partial t}\Psi(\mathbf{r},t) = \hat{H}\Psi(\mathbf{r},t)
$$

## Fractions and Binomials

$$
\frac{n!}{k!(n-k)!} = \binom{n}{k}
$$

Complex fraction:

$$
\frac{\frac{1}{x}+\frac{1}{y}}{y-z}
$$

## Matrices

$$
\begin{bmatrix}
1 & 2 & 3 \\
4 & 5 & 6 \\
7 & 8 & 9
\end{bmatrix}
$$

Identity matrix:

$$
I = \begin{pmatrix}
1 & 0 & 0 \\
0 & 1 & 0 \\
0 & 0 & 1
\end{pmatrix}
$$

## Greek Letters

$$
\alpha, \beta, \gamma, \delta, \epsilon, \zeta, \eta, \theta, \iota, \kappa, \lambda, \mu, \nu, \xi, \pi, \rho, \sigma, \tau, \upsilon, \phi, \chi, \psi, \omega
$$

Uppercase:

$$
\Gamma, \Delta, \Theta, \Lambda, \Xi, \Pi, \Sigma, \Upsilon, \Phi, \Psi, \Omega
$$

## Summations and Products

$$
\sum_{i=1}^{n} i = \frac{n(n+1)}{2}
$$

$$
\prod_{i=1}^{n} i = n!
$$

$$
\sum_{n=1}^{\infty} \frac{1}{n^2} = \frac{\pi^2}{6}
$$

## Limits

$$
\lim_{x \to \infty} \frac{1}{x} = 0
$$

$$
\lim_{h \to 0} \frac{f(x+h) - f(x)}{h} = f'(x)
$$

## Integrals

Definite integral:

$$
\int_{a}^{b} f(x) \, dx
$$

Multiple integral:

$$
\iiint_V f(x,y,z) \, dV
$$

## Derivatives

$$
\frac{d}{dx} (x^n) = nx^{n-1}
$$

Partial derivative:

$$
\frac{\partial f}{\partial x}
$$

## Trigonometry

$$
\sin^2(x) + \cos^2(x) = 1
$$

$$
e^{ix} = \cos(x) + i\sin(x)
$$

## Calculus

Taylor series:

$$
f(x) = \sum_{n=0}^{\infty} \frac{f^{(n)}(a)}{n!}(x-a)^n
$$

Fundamental theorem of calculus:

$$
\int_{a}^{b} f'(x) \, dx = f(b) - f(a)
$$

## Complex Formulas

Maxwell's equations:

$$
\begin{aligned}
\nabla \cdot \mathbf{E} &= \frac{\rho}{\epsilon_0} \\
\nabla \cdot \mathbf{B} &= 0 \\
\nabla \times \mathbf{E} &= -\frac{\partial \mathbf{B}}{\partial t} \\
\nabla \times \mathbf{B} &= \mu_0\mathbf{J} + \mu_0\epsilon_0\frac{\partial \mathbf{E}}{\partial t}
\end{aligned}
$$

Navier-Stokes equation:

$$
\rho \left( \frac{\partial \mathbf{v}}{\partial t} + \mathbf{v} \cdot \nabla \mathbf{v} \right) = -\nabla p + \mu \nabla^2 \mathbf{v} + \mathbf{f}
$$

## Statistics

Normal distribution:

$$
f(x) = \frac{1}{\sigma\sqrt{2\pi}} e^{-\frac{1}{2}\left(\frac{x-\mu}{\sigma}\right)^2}
$$

Bayes' theorem:

$$
P(A|B) = \frac{P(B|A)P(A)}{P(B)}
$$

## Set Theory

$$
A \cup B, \quad A \cap B, \quad A \setminus B, \quad A \subseteq B
$$

$$
\forall x \in \mathbb{R}, \exists y \in \mathbb{R} : y > x
$$

## Logic

$$
(p \land q) \lor (\neg p \lor \neg q)
$$

## Special Symbols

$$
\infty, \nabla, \partial, \hbar, \ell, \aleph, \Im, \Re
$$

## Mixed Inline and Block

The area of a circle with radius $r$ is given by:

$$
A = \pi r^2
$$

And the circumference is $C = 2\pi r$.

## Edge Cases

Empty formula: $$$$

Single character: $x$

Very long inline formula: $f(x) = a_0 + a_1x + a_2x^2 + a_3x^3 + a_4x^4 + a_5x^5 + a_6x^6 + a_7x^7 + a_8x^8 + a_9x^9 + a_{10}x^{10}$

---

If all formulas above render correctly, KaTeX integration is working perfectly!
