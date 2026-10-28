export function PageLoader() {
    return (
        <main
            className="page-loader"
            role="status"
            aria-live="polite">
            <span className="page-loader__spinner" />

            <p>Loading the app...</p>
        </main>
    )
}