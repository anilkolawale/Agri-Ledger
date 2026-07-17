import React from "react";

// Wraps the whole app so a broken component shows a readable error + component
// stack in the UI (and console) instead of a blank screen — makes "Element type
// is invalid" style errors immediately actionable: the componentStack below
// names the exact file/component tree where it happened.
export default class ErrorBoundary extends React.Component {
  constructor(props) {
    super(props);
    this.state = { error: null, info: null };
  }

  static getDerivedStateFromError(error) {
    return { error };
  }

  componentDidCatch(error, info) {
    this.setState({ info });
    // eslint-disable-next-line no-console
    console.error("AgriLedger render error:", error, info?.componentStack);
  }

  render() {
    if (this.state.error) {
      return (
        <div style={{ padding: 24, fontFamily: "monospace", whiteSpace: "pre-wrap" }}>
          <h2 style={{ color: "#c62828" }}>Something broke while rendering AgriLedger</h2>
          <p><strong>{String(this.state.error?.message || this.state.error)}</strong></p>
          {this.state.info?.componentStack && (
            <>
              <p>Component stack (this tells you exactly which file/component failed):</p>
              <pre style={{ background: "#f5f5f5", padding: 12, overflowX: "auto" }}>
                {this.state.info.componentStack}
              </pre>
            </>
          )}
        </div>
      );
    }
    return this.props.children;
  }
}
