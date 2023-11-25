<!-- routify:options preload="proximity" -->

<script>
    function goToAccount() {
        history.pushState({}, null, '/account');
    }
</script>



<div class="d-flex flex-column h-100">
    <nav class="top-row p-3 border-bottom navbar shadow">
        <div class="container-fluid d-flex gap-3">
            <a class="navbar-brand nav-link text-dark" href="/">MyWords</a>
            <div class="flex-grow-1"></div>
            <!-- <a class="nav-link text-dark" href="login">Login</a>
            <a class="nav-link text-dark" href="signup">Sign Up</a> -->
            <!-- <button on:click={goToAccount}>Account</button> -->

            <div class="dropdown">
                <button class="btn" data-bs-toggle="dropdown" aria-expanded="false">
                  Dropdown button
                </button>
                <ul class="dropdown-menu dropdown-menu-end">
                  <li><button class="dropdown-item" href="#" on:click={goToAccount}>My Account</button></li>
                </ul>
              </div>
        </div>
    </nav>
    <main class="flex-grow-1 px-4 py-4">
        <slot />
    </main>
    <!-- <div class="footer p-3">
    </div> -->
</div>